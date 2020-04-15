/*
	Peter Francis
	available to use according to UM_AI/LICENSE
*/

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

// Temporary using
using SensorToolkit;

namespace UM_AI
{

	public enum SummingType {WeightedAverage, Prioritized, Dithered}

	public enum DecelerationType : int {None=0, Fast=1, Normal=2, Slow=3}

	[RequireComponent(typeof(Rigidbody2D))]
	public partial class SteeringRig : MonoBehaviour
	{
		const float EPSILON = 0.01f;
		const float TWOPI = 2f * Mathf.PI;

		#region Properties

		[Header("Movement")]
		[Tooltip("The maximum movement speed that will be applied in a forwards direction.")]
		public float maxSpeed;
		[Tooltip("The maximum turning speed that will be applied.")]
		public float turnSpeed;
		[Tooltip("The maximum movement speed that will be applied in a sideways or backwards direction.")]
		public float strafeSpeed;
		[Tooltip("Type of deceleration to use.")]
		public DecelerationType deceleration = DecelerationType.Slow;

		[Header("Summing type")]
		[SerializeField,Tooltip("Type of steering vector summing calculation to use.")]
		SummingType _summingType = SummingType.WeightedAverage;
		public SummingType SummingType
		{
			get => _summingType;
			set
			{
				_summingType = value;
				switch (value)
				{
					case SummingType.WeightedAverage:
						summingCalculate = CalculateWeightedSum;
						break;
					case SummingType.Prioritized:
						summingCalculate = CalculatePrioritized;
						break;
					case SummingType.Dithered:
						summingCalculate = CalculateDithered;
						break;
					default:
						summingCalculate = ()=>Vector2.zero;
						break;
				}
			}
		}

		[Header("Avoidance")]
		[Range(0.1f,10f), Tooltip("Sensitivity of distance steered to obstacles.")]
		public float avoidanceSensitivity = 1f;
		[Tooltip("The rig won't try to steer around objects in this list.")]
		public List<GameObject> _ignoreList;
		public List<GameObject> IgnoreList
		{
			get => _ignoreList;
			set
			{
				_ignoreList = value;
				foreach(RaySensor2D rs in sensors) rs.IgnoreList = _ignoreList;
			}
		}

		[Header("Look moving smoothing")]
		[Tooltip("Whether to rotate to face the direction of movement.")]
		public bool lookMovingDirection = true;
        public bool smoothing = true;
        public int bufferSize = 5;

		Queue<Vector2> velocityBuffer = new Queue<Vector2>();
		Vector2 steeringForce;
		RaySensor2D[] sensors;
		Rigidbody2D RB;
		Vector2 targetPos;
		Rigidbody2D target1;
		Rigidbody2D target2;
		Rigidbody2D[] neighbors;
		delegate Vector2 SummingMethod();
		SummingMethod summingCalculate;
		bool trackingToDestinationPosition = false;

		private Transform _destinationTransform;
		public Transform DestinationTransform
		{
			get => _destinationTransform;
			set
			{
				if (value!=null) IgnoreList.Add(value.gameObject);
				else if (_destinationTransform != null) IgnoreList.Remove(_destinationTransform.gameObject);
				_destinationTransform = value;
				target1 = _destinationTransform?.GetComponent<Rigidbody2D>();
			}
		}

		public Vector2 Destination
		{
			get => DestinationTransform != null ? (Vector2)DestinationTransform.position : targetPos;
			set
			{
				if (DestinationTransform != null)
				{
					Debug.LogWarning("Cannot set Destination while DestinationTransform is not Null.");
				}
				else
				{
					targetPos = value;
					trackingToDestinationPosition = true;
				}
			}
		}

		#endregion

		Vector2 Heading => steeringForce.normalized;
		// NOTE: Forward for 2D
		Vector2 Forward => (Vector2)transform.right;

		void Awake()
		{
			RB = GetComponent<Rigidbody2D>();
			// TODO: use original sensors
			sensors = GetComponentsInChildren<RaySensor2D>();
			IgnoreList.AddRange(GetComponentsInChildren<Collider2D>(includeInactive:true).Select(x=>x.gameObject));
			for (int i=0; i<sensors.Length; ++i)
			{
				sensors[i].IgnoreList = IgnoreList;
				sensors[i].SensorUpdateMode = RaySensor2D.UpdateMode.Manual;
			}
			SummingType = _summingType;
			float theta = Random.value * TWOPI;
			wanderTarget = new Vector2(Mathf.Cos(theta)*wanderRadius,Mathf.Sin(theta)*wanderRadius);
		}

		void Update()
		{
			// reset the steering force
			steeringForce = Vector2.zero;
			steeringForce = summingCalculate();
			if (lookMovingDirection) FaceDirectionKinematic(steeringForce);
			// Lerp the dot product of the direction facing to the direction moving,
			// this will interpolate between the strafing force and the moving force.
			float forwardDotVel = Vector3.Dot(RB.transform.up, Heading);
			steeringForce *= Mathf.Lerp(strafeSpeed, maxSpeed, Mathf.Clamp(forwardDotVel,0f,1f));
			Steer(steeringForce);
		}

		void Steer(Vector2 direction)
		{
			RB.velocity += direction * Time.deltaTime;
			RB.velocity = Vector2.ClampMagnitude(RB.velocity,maxSpeed);
		}

		void FaceDirectionKinematic(Vector2 direction)
		{
            if (smoothing)
            {
                if (velocityBuffer.Count >= bufferSize)
                {
                    velocityBuffer.Dequeue();
                }

                velocityBuffer.Enqueue(RB.velocity);

                direction = Vector2.zero;

                foreach (Vector2 v in velocityBuffer)
                {
                    direction += v;
                }

                direction /= velocityBuffer.Count;
            }
			if (direction.sqrMagnitude > EPSILON)
			{
				float toRotation = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg);
				float newRot = Mathf.LerpAngle(RB.rotation, toRotation, Time.deltaTime * turnSpeed);
				RB.MoveRotation(newRot);
				// RB.transform.rotation = Quaternion.RotateTowards(RB.transform.rotation,
				// 							Quaternion.Euler(Vector3.forward*toRotation),turnSpeed*Time.deltaTime);
			}
		}

		bool AccumulateForce(Vector2 forceToAdd)
		{
			// calculate how much steering force remains to be used by this vehicle
			float magnitude = maxSpeed - steeringForce.magnitude;

			// return false if there is no more force left to use
			if (magnitude <= 0f) return false;

			//calculate the magnitude of the force we want to add
			float magnitudeToAdd = forceToAdd.magnitude;
			
			// if the magnitude of the sum of ForceToAdd and the running total
			// does not exceed the maximum force available to this vehicle, just
			// add together. Otherwise add as much of the ForceToAdd vector is
			// possible without going over the max.
			if (magnitudeToAdd < magnitude)
			{
				steeringForce += forceToAdd;
			}
			else
			{
				// add to the steering force
				steeringForce += (forceToAdd.normalized * magnitude); 
			}

			return true;
		}

		//---------------------- CalculateWeightedSum ----------------------------
		//
		//  this simply sums up all the active behaviors X their weights and 
		//  truncates the result to the max available steering force before 
		//  returning
		//------------------------------------------------------------------------
		Vector2 CalculateWeightedSum()
		{        
			if (HasFlag(SteeringType.Avoidance))
			{
				steeringForce += Avoidance() * weights.avoidance;
			}
			if (HasFlag(SteeringType.Evade) && target1 != null)
			{
				// Debug.Assert(target1!=null, "Evade target not assigned!");
				steeringForce += Evade(target1) * weights.evade;
			}

			// FLOCKING
			if (HasFlag(SteeringType.Separation))
			{
				steeringForce += Separation(neighbors) * weights.separation;
			}
			if (HasFlag(SteeringType.Alignment))
			{
				steeringForce += Alignment(neighbors) * weights.alignment;
			}
			if (HasFlag(SteeringType.Cohesion))
			{
				steeringForce += Cohesion(neighbors) * weights.cohesion;
			}

			if (HasFlag(SteeringType.Wander))
			{
				steeringForce += Wander() * weights.wander;
			}

			if (HasFlag(SteeringType.Seek))
			{
				steeringForce += Seek(targetPos) * weights.seek;
			}
			if (HasFlag(SteeringType.Flee))
			{
				steeringForce += Flee(targetPos) * weights.flee;
			}

			if (HasFlag(SteeringType.Arrive))
			{
				steeringForce += Arrive(targetPos, deceleration) * weights.arrive;
			}

			if (HasFlag(SteeringType.Pursuit) && target1 != null)
			{
				// Debug.Assert(target1!=null,"Pursuit target not assigned!");
				steeringForce += Pursuit(target1) * weights.pursuit;
			}

			if (HasFlag(SteeringType.Interpose) && target1 != null && target2 != null)
			{
				// Debug.Assert(target1!=null && target2!=null,"Interpose agents not assigned!");
				steeringForce += Interpose(target1,target2,deceleration) * weights.interpose;
			}

			return Vector2.ClampMagnitude(steeringForce,maxSpeed);
		}

		//---------------------- CalculatePrioritized ----------------------------
		//
		//  this method calls each active steering behavior in order of priority
		//  and acumulates their forces until the max steering force magnitude
		//  is reached, at which time the function returns the steering force 
		//  accumulated to that  point
		//------------------------------------------------------------------------
		Vector2 CalculatePrioritized()
		{
			Vector2 force;
			
			if (HasFlag(SteeringType.Avoidance))
			{
				force = Avoidance() * weights.avoidance;
				if (!AccumulateForce(force)) return steeringForce;
			}
			if (HasFlag(SteeringType.Evade) && target1!=null)
			{
				// Debug.Assert(target1!=null, "Evade target not assigned!");
				force = Evade(target1) * weights.evade;
				if (!AccumulateForce(force)) return steeringForce;
			}
			if (HasFlag(SteeringType.Flee))
			{
				force = Flee(targetPos) * weights.flee;
				if (!AccumulateForce(force)) return steeringForce;
			}

			if (HasFlag(SteeringType.Separation))
			{
				force = Separation(neighbors) * weights.separation;
				if (!AccumulateForce(force)) return steeringForce;
			}
			if (HasFlag(SteeringType.Alignment))
			{
				force = Alignment(neighbors) * weights.alignment;
				if (!AccumulateForce(force)) return steeringForce;
			}
			if (HasFlag(SteeringType.Cohesion))
			{
				force = Cohesion(neighbors) * weights.cohesion;
				if (!AccumulateForce(force)) return steeringForce;
			}

			if (HasFlag(SteeringType.Seek))
			{
				force = Seek(targetPos) * weights.seek;
				if (!AccumulateForce(force)) return steeringForce;
			}
			if (HasFlag(SteeringType.Arrive))
			{
				force = Arrive(targetPos, deceleration) * weights.arrive;
				if (!AccumulateForce(force)) return steeringForce;
			}
			if (HasFlag(SteeringType.Wander))
			{
				force = Wander() * weights.wander;
				if (!AccumulateForce(force)) return steeringForce;
			}
			if (HasFlag(SteeringType.Pursuit) && target1!=null)
			{
				// Debug.Assert(target1!=null,"Pursuit target not assigned!");
				force = Pursuit(target1) * weights.pursuit;
				if (!AccumulateForce(force)) return steeringForce;
			}
			if (HasFlag(SteeringType.Interpose) & target1!=null && target2!=null)
			{
				// Debug.Assert(target1!=null && target2!=null,"Interpose agents not assigned!");
				force = Interpose(target1, target2, deceleration) * weights.interpose;
				if (!AccumulateForce(force)) return steeringForce;
			}

			return steeringForce;
		}

		// TODO
		Vector2 CalculateDithered() { return Vector2.zero; }

		#if UNITY_EDITOR
		void OnDrawGizmos()
		{
			if (Application.isPlaying)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawRay(transform.position,Heading*20f);
			}
		}
		#endif
	}
}
