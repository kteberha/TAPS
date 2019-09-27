using UnityEngine;
using UModules;

public struct PosRot
{
	Vector3 position;
	Quaternion rotation;

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

public class Tentacle : MonoBehaviour
{

	[Readonly]
	public Transform RootJoint;
	[Readonly]
	public ArmJoint[] Joints;
	[Readonly]
	public float[] Solution;

	public Transform Destination;
	[Tooltip("Mininum distance from destination")]
	public float DistanceFromDestination;
	private Vector3 target;

	[Range(0, 1f)]
	public float DeltaGradient = 0.1F;
	[Range(0, 100f)]
	public float LearningRate = 0.1F;

	[Range(0, 0.25f)]
	public float StopThreshold = 0.1F;
	[Range(0, 10f)]
	public float SlowdownThreshold = 0.25F;

	[Range(0, 10)]
	public float OrientationWeight = 0.5F;
	[Range(0, 10)]
	public float TorsionWeight = 0.5F;
	public Vector3 TorsionPenality = new Vector3(1, 0, 0);

	void Awake()
	{
		Joints = RootJoint.GetComponentsInChildren<ArmJoint>();
		Solution = new float[Joints.Length];
	}

	void Update()
	{
		Vector3 direction = (Destination.position - transform.position).normalized;
		target = Destination.position - direction * DistanceFromDestination;
		if (Comfort(target, Solution) > StopThreshold) ApproachTarget(target);
	}

	private void ApproachTarget(Vector3 target)
	{
		for (int i = Joints.Length - 1; i >= 0; i--)
		{
			float error = Comfort(target, Solution);
			float slowdown = Mathf.Clamp01((error - StopThreshold) / (SlowdownThreshold - StopThreshold));
			float gradient = CalculateGradient(target, Solution, i, DeltaGradient);
			Solution[i] -= LearningRate * gradient * slowdown;
			Solution[i] = Joints[i].ClampAngle(Solution[i]);
			if (Comfort(target, Solution) <= StopThreshold) break;
		}
		for (int i = 0; i < Joints.Length-1; i++)
		{
			Joints[i].MoveArm(Solution[i]);
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
		PosRot result = ForwardKinematics(Solution);
		float torsion = 0;
		for (int i = 0; i < solution.Length; i++)
		{
			torsion += Mathf.Abs(solution[i]) * TorsionPenality.x;
			torsion += Mathf.Abs(solution[i]) * TorsionPenality.y;
			torsion += Mathf.Abs(solution[i]) * TorsionPenality.z;
		}
		return Vector3.Distance(target, result)
			+ Mathf.Abs(Quaternion.Angle(result, Destination.rotation)/180F) * OrientationWeight
			+ (torsion / solution.Length) * TorsionWeight;
	}

	public float DistanceFromTarget(Vector3 target, float[] solution)
	{
		Vector3 point = ForwardKinematics(solution);
		return Vector3.Distance(point, target);
	}

	public PosRot ForwardKinematics(float[] solution)
	{
		Vector3 prevPoint = Joints[0].transform.position;
		Quaternion rotation = transform.rotation;
		for (int i = 1; i < Joints.Length; i++)
		{
			rotation *= Quaternion.AngleAxis(solution[i - 1], Joints[i - 1].Axis);
			Vector3 nextPoint = prevPoint + rotation * Joints[i].StartOffset;
			prevPoint = nextPoint;
		}
		return new PosRot(prevPoint, rotation);
	}
}
