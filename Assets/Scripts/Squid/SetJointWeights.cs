using UnityEngine;

public class SetJointWeights : MonoBehaviour
{
	public Vector2 minAngle;
	public Vector2 maxAngle;

	public float minSpeed;
	public float maxSpeed;
	
	public Vector3[] axes = {Vector3.forward};

	private ArmJoint[] joints;

	//private static readonly Vector3[] axes = {Vector3.right,Vector3.up,Vector3.forward};

	public void Awake()
	{
		SetJointAngles();
	}

	public void SetJointAngles()
	{
		joints = GetComponentsInChildren<ArmJoint>();
		for (int i=0; i<joints.Length; i++)
		{
			float t = (float)i / (joints.Length - 1);

			joints[i].MinAngle = Mathf.Lerp(minAngle.x, minAngle.y, t);
			joints[i].MaxAngle = Mathf.Lerp(maxAngle.x, maxAngle.y, t);

			joints[i].Speed = Mathf.Lerp(minSpeed, maxSpeed, t);

			joints[i].Axis = axes[i % axes.Length];
		}
	}
}