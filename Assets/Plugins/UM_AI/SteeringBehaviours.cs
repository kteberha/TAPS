using System.Collections.Generic;
using UnityEngine;
using UModules;
using Random=UnityEngine.Random;

namespace UM_AI
{
	[System.Flags]
	public enum SteeringType
	{
		None		= 0,		// 0
		Seek		= 1<<0,		// 1
		Flee		= 1<<1,		// 2
		Arrive		= 1<<2,		// 3
		Wander		= 1<<3,		// 4
		Cohesion	= 1<<4,		// 5
		Separation	= 1<<5,		// 6
		Alignment	= 1<<6,		// 7
		Avoidance	= 1<<7,		// 8
		Pursuit		= 1<<8,		// 9
		Evade		= 1<<9,		// 10
		Interpose	= 1<<10		// 11
		//Hide		= 1<<11		// 12
		//Flock		= 1<<12		// 13
	};
	[System.Serializable]
	public struct SteeringBehaviourWeights
	{
		public float separation;
		public float cohesion;
		public float alignment;
		public float wander;
		public float avoidance;
		public float seek;
		public float flee;
		public float arrive;
		public float pursuit;
		public float interpose;
		public float evade;
		public float hide;
	}

	public partial class SteeringRig : MonoBehaviour
	{
		[EnumFlag,SerializeField]
		SteeringType stFlags = SteeringType.None;
		[SerializeField]
		SteeringBehaviourWeights weights = new SteeringBehaviourWeights();
		
		[Range(0f,2f),SerializeField]
		float wanderJitter = 1f;
		[Range(EPSILON,100f),SerializeField]
		float wanderRadius = 10f;
		[Range(EPSILON,100f),SerializeField]
		float wanderDistance = 15f;

		//the current position on the wander circle the agent is
		//attempting to steer towards
		Vector2 wanderTarget; 
		
		public bool HasFlag(SteeringType st) => (stFlags & st) == st;
		public void SetFlag(SteeringType st) => stFlags |= st;
		public void UnsetFlag(SteeringType st) => stFlags &= ~st;

		/// <summary>
		/// Returns steering force directly to target
		/// </summary>
		/// <param name="targetPos">Target Position</param>
		Vector2 Seek(Vector2 targetPos)
		{
			Vector2 desiredVelocity = (targetPos - RB.position).normalized * maxSpeed;
			return (desiredVelocity - RB.velocity);
		}

		/// <summary>
		/// Opposite of Seek, returns steering force directly opposite to target
		/// </summary>
		/// <param name="targetPos"></param>
		/// <returns></returns>
		Vector2 Flee(Vector2 targetPos)
		{
			//only flee if the target is within 'panic distance'. Work in distance squared space
			// const float PANICDISTANCESQ = 100f * 100f;
			// if (Vector2.DisanceSq(RB.position, target) > PANICDISTANCESQ)
			// {
			// 	return Vector2.zero;
			// }

			Vector2 desiredVelocity = (RB.position - targetPos).normalized * maxSpeed;

			return (desiredVelocity - RB.velocity);
		}

		//--------------------------- Arrive -------------------------------------
		//
		//  This behavior is similar to seek but it attempts to arrive at the
		//  target with a zero velocity
		//------------------------------------------------------------------------
		Vector2 Arrive(Vector2 targetPos, DecelerationType decel)
		{
			Vector2 toTarget = targetPos - RB.position;

			//calculate the distance to the target
			float dist = toTarget.magnitude;

			if (dist > 0)
			{
				//because Deceleration is enumerated as an int, this value is required
				//to provide fine tweaking of the deceleration...
				const float DECELTWEAK = 0.3f;

				//calculate the speed required to reach the target given the desired
				//deceleration
				float speed =  dist / ((float)decel * DECELTWEAK);     

				//make sure the velocity does not exceed the max
				speed = Mathf.Min(speed, maxSpeed);

				//from here proceed just like Seek except we don't need to normalize 
				//the ToTarget vector because we have already gone to the trouble
				//of calculating its length: dist. 
				Vector2 desiredVelocity =  toTarget * speed / dist;

				return (desiredVelocity - RB.velocity);
			}

			return Vector2.zero;
		}

		//------------------------------ Pursuit ---------------------------------
		//
		//  this behavior creates a force that steers the agent towards the 
		//  evader
		//------------------------------------------------------------------------
		Vector2 Pursuit(Rigidbody2D target)
		{
			const float DEGLIMIT = -0.95f; //acos(0.95)=18 degs
			//if the evader is ahead and facing the agent then we can just seek for the evader's current position.
			Vector2 toTarget = target.position - RB.position;
			Vector2 heading = RB.velocity.normalized;

			float relativeHeading = Vector2.Dot(heading,target.velocity.normalized);

			if ((Vector2.Dot(toTarget,heading) > 0f) && (relativeHeading < DEGLIMIT))  
			{
				return Seek(target.position);
			}

			//Not considered ahead so we predict where the evader will be.
			
			//the lookahead time is propotional to the distance between the evader
			//and the pursuer; and is inversely proportional to the sum of the agent's velocities
			float lookAheadTime = toTarget.magnitude / (maxSpeed + RB.velocity.magnitude);
			
			//now seek to the predicted future position of the evader
			return Seek(target.position + target.velocity * lookAheadTime);
		}

		//----------------------------- Evade ------------------------------------
		//
		//  similar to pursuit except the agent Flees from the estimated future
		//  position of the pursuer
		//------------------------------------------------------------------------
		Vector2 Evade(Rigidbody2D target)
		{
			// Not necessary to include the check for facing direction this time 

			Vector2 toTarget = target.position - RB.position;

			// Evade only consider pursuers within a 'threat range'
			// const float THREATRANGE = 100f * 100f;
			// if (toTarget.sqrMagnitude > THREATRANGE) return Vector2.zero;
			
			//the lookahead time is propotional to the distance between the pursuer
			//and the pursuer; and is inversely proportional to the sum of the
			//agents' velocities
			float lookAheadTime = toTarget.magnitude / (maxSpeed + target.velocity.magnitude);
			
			//now flee away from predicted future position of the pursuer
			return Flee(target.position + target.velocity * lookAheadTime);
		}

		//--------------------------- Wander -------------------------------------
		//
		//  This behavior makes the agent wander about randomly
		//------------------------------------------------------------------------
		Vector2 Wander()
		{ 
			//this behavior is dependent on the update rate, so this line must
			//be included when using time independent framerate.
			float jitterTimeSlice = wanderJitter * Time.deltaTime;

			//first, add a small random vector to the target's position
			wanderTarget += new Vector2(Random.Range(-1f,1f)*jitterTimeSlice, Random.Range(-1f,1f)*jitterTimeSlice);

			//reproject this new vector back on to a unit circle
			wanderTarget.Normalize();

			//increase the length of the vector to the same as the radius
			//of the wander circle
			wanderTarget *= wanderRadius;

			//move the target into a position WanderDist in front of the agent
			Vector2 target = RB.position + Side * wanderDistance + wanderTarget;

			//and steer towards it
			return target - RB.position;
		}

		//---------------------------- Alignment ---------------------------------
		//
		//  returns a force that attempts to align this agents heading with that
		//  of its neighbors
		//------------------------------------------------------------------------
		Vector2 Alignment(Rigidbody2D[] neighbors)
		{
			//used to record the average heading of the neighbors
			Vector2 averageHeading = Vector2.zero;

			//used to count the number of vehicles in the neighborhood
			int neighborCount = neighbors.Length;
			if (neighborCount == 0) return averageHeading;

			//iterate through all the tagged vehicles and sum their heading vectors  
			for (uint i=0; i<neighborCount; ++i)
			{
				averageHeading += neighbors[i].velocity.normalized;
			}

			//if the neighborhood contained one or more vehicles, average their
			//heading vectors.
			if (neighborCount > 0)
			{
				averageHeading /= neighborCount;
				averageHeading -= RB.velocity.normalized;
			}
			
			return averageHeading;
		}

		//---------------------------- Separation --------------------------------
		//
		// this calculates a force repelling from the other neighbors
		//------------------------------------------------------------------------
		Vector2 Separation(Rigidbody2D[] neighbors)
		{  
			Vector2 steeringForce = Vector2.zero;
			int neighborCount = neighbors.Length;
			if (neighborCount == 0) return steeringForce;

			for (uint i=0; i<neighborCount; ++i)
			{
				Vector2 between = RB.position - neighbors[i].position;
				steeringForce += between.normalized/between.magnitude;
			}

			return steeringForce;
		}

		//-------------------------------- Cohesion ------------------------------
		//
		//  returns a steering force that attempts to move the agent towards the
		//  center of mass of the agents in its immediate area
		//------------------------------------------------------------------------
		Vector2 Cohesion(Rigidbody2D[] neighbors)
		{
			Vector2 centerOfMass = Vector2.zero;
			int neighborCount = neighbors.Length;
			if (neighborCount == 0) return centerOfMass;

			// iterate through the neighbors and sum up all the position vectors
			for (uint i=0; i<neighborCount; ++i)
			{
				centerOfMass += neighbors[i].position;
			}

			if (neighborCount > 0)
			{
				//the center of mass is the average of the sum of positions
				centerOfMass /= neighborCount;
			}

			//the magnitude of cohesion is usually much larger than separation or
			//allignment so it usually helps to normalize it.
			return Seek(centerOfMass).normalized;
		}

		//--------------------------- Interpose ----------------------------------
		//
		//  Given two agents, this method returns a force that attempts to 
		//  position the vehicle between them
		//------------------------------------------------------------------------
		Vector2 Interpose(Rigidbody2D a, Rigidbody2D b, DecelerationType decel)
		{
			//first we need to figure out where the two agents are going to be at 
			//time T in the future. This is approximated by determining the time
			//taken to reach the mid way point at the current time at at max speed.
			Vector2 midPoint = (a.position + b.position) / 2f;

			float timeToReachMidPoint = Vector2.Distance(RB.position,midPoint) / this.maxSpeed;

			//now we have T, we assume that agent A and agent B will continue on a
			//straight trajectory and extrapolate to get their future positions
			Vector2 aPos = a.position + a.velocity * timeToReachMidPoint;
			Vector2 bPos = b.position + b.velocity * timeToReachMidPoint;

			//calculate the mid point of these predicted positions
			midPoint = (aPos + bPos) / 2f;

			//then steer to Arrive at it
			return Arrive(midPoint, decel);
		}

		Vector2 Avoidance()
		{
			Vector2 rf = Vector2.zero;
			for (int i = 0; i < sensors.Length; i++)
			{
				var s = sensors[i];
				s.IgnoreList = ignoreList;
				s.Pulse();
				if (!s.IsObstructed) continue;
				// 0 when unobstructed, 1 when touching
				float obsRatio = Mathf.Pow(1f-(s.ObstructionRayHit.distance/s.Length), 1f/avoidanceSensitivity);
				rf += obsRatio * s.ObstructionRayHit.normal;
			}
			float rfMag = rf.magnitude;
			if (rfMag > maxAvoidanceLength)
			{
				return rf * maxAvoidanceLength;
			}
			return rf * rfMag;
		}

	}
}