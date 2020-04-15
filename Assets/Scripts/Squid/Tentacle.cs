using UnityEngine;
using UModules;

public class Tentacle : MonoBehaviour
{
	const float EPSILON = 0.001f;
	const string ROOTJOINTNAME = "Armature/Root";

	#region Properties

	[Header("Target")]

	[SerializeField]
	Transform _target;
	public Transform Target
	{
		get => _target;
		set
		{
			_target = value;
			dynamicBone.enabled = _target==null;
		}
	}

	[Readonly,SerializeField]
	private Vector3 destination;

	[Header("Solution Parameters")]

	[Tooltip("Mininum distance from target")]
	public float mininumDistance;
	[Range(EPSILON,1f)]
	public float deltaGradient = 0.1f;
	[Range(EPSILON,100f)]
	public float learningRate = 0.1f;
	[Range(0f,0.25f)]
	public float stopThreshold = 0.1f;
	[Range(0f,10f)]
	public float slowdownThreshold = 0.25f;
	[Range(0f,10f)]
	public float orientationWeight = 0.5f;
	[Range(0f,10f)]
	public float torsionWeight = 0.5f;
	public Vector3 torsionPenality = Vector3.right;
	[Readonly]
	public float[] solution;

	[Header("Armature")]

	public Transform rootJoint;
	[Readonly]
	public ArmJoint[] joints;
	[Readonly]
	public Transform endBone;

	[Header("Components")]

	[Readonly]
	public DynamicBone dynamicBone;
	[Readonly]
	public TentacleHand hand;

	public Transform HeldObject => hand?.heldObject;
	public bool Holding => hand?.Holding??false;

	#endregion

	public void ToggleDB() => dynamicBone.enabled = !dynamicBone.enabled;
	public void ToggleCollider() => hand.ToggleCollider();

	public void HandAttach(Transform t) => hand?.HandAttach(t);
	public void HandDetach() => hand?.HandDetach(); 

	void Awake()
	{
		//RootJoint = transform.Find(ROOTJOINTNAME);
		joints = rootJoint.GetComponentsInChildren<ArmJoint>();
		dynamicBone = GetComponent<DynamicBone>();
		endBone = joints[joints.Length-1].transform;
		hand = endBone.GetComponent<TentacleHand>();
		hand.triggerEnter.AddListener(HandCollisionHandler);
		solution = new float[joints.Length];
	}

	void Update()
	{
		if (Target != null)
		{
			Vector3 direction = (Target.position - transform.position).normalized;
			destination = Target.position - direction * mininumDistance;
			if (Comfort(destination, solution) > stopThreshold) ApproachTarget(destination);
		}
	}

	void HandCollisionHandler(Collider2D other)
	{
		if (other.transform == Target)
		{
			HandAttach(other.transform);
		}
	}

	private void ApproachTarget(Vector3 target)
	{
		for (int i = joints.Length - 1; i >= 0; i--)
		{
			float error = Comfort(target, solution);
			float slowdown = Mathf.Clamp01((error - stopThreshold) / (slowdownThreshold - stopThreshold));
			float gradient = CalculateGradient(target, solution, i, deltaGradient);
			solution[i] -= learningRate * gradient * slowdown;
			solution[i] = joints[i].ClampAngle(solution[i]);
			if (Comfort(target, solution) <= stopThreshold) break;
		}
		for (int i = 0; i < joints.Length-1; i++)
		{
			joints[i].MoveArm(solution[i]);
		}
	}

	public float CalculateGradient(Vector3 target, float[] solution, int i, float delta)
	{

		float solutionAngle = solution[i];
		float f_x = Comfort(target, solution);
		solution[i] += delta;

		float f_x_plus_h = Comfort(target, solution);
		float gradient = (f_x_plus_h - f_x) / delta;
		solution[i] = solutionAngle;

		return gradient;
	}

	public float Comfort(Vector3 target, float[] solution)
	{
		PosRot result = ForwardKinematics(this.solution);
		float torsion = 0;
		for (int i = 0; i < solution.Length; i++)
		{
			torsion += Mathf.Abs(solution[i]) * torsionPenality.x;
			torsion += Mathf.Abs(solution[i]) * torsionPenality.y;
			torsion += Mathf.Abs(solution[i]) * torsionPenality.z;
		}
		return Vector3.Distance(target, result)
			+ Mathf.Abs(Quaternion.Angle(result, this.Target.rotation)/180F) * orientationWeight
			+ (torsion / solution.Length) * torsionWeight;
	}

	public float DistanceFromTarget(Vector3 target, float[] solution)
	{
		Vector3 point = ForwardKinematics(solution);
		return Vector3.Distance(point, target);
	}

	public PosRot ForwardKinematics(float[] solution)
	{
		Vector3 prevPoint = joints[0].transform.position;
		Quaternion rotation = transform.rotation;
		for (int i = 1; i < joints.Length; i++)
		{
			rotation *= Quaternion.AngleAxis(solution[i - 1], ToVector(joints[i - 1].axis));
			Vector3 nextPoint = prevPoint + rotation * joints[i].startOffset;
			prevPoint = nextPoint;
		}
		return new PosRot(prevPoint, rotation);
	}

	public static Vector3 ToVector(Axis axis)
	{
		switch (axis)
		{
			case Axis.X: return Vector3.right;
			case Axis.Y: return Vector3.up;
			case Axis.Z: return Vector3.forward;
			default: return Vector3.zero;
		}
	}
}

public struct PosRot
{
	public Vector3 position;
	public Quaternion rotation;

	public PosRot(Vector3 position, Quaternion rotation)
	{
		this.position = position;
		this.rotation = rotation;
	}

	public static implicit operator Vector3(PosRot pr)
	{
		return pr.position;
	}

	public static implicit operator Quaternion(PosRot pr)
	{
		return pr.rotation;
	}
}
