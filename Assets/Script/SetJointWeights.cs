using UnityEngine;

public class SetJointWeights : MonoBehaviour
{
	public Vector2 MinAngle;
	public Vector2 MaxAngle;
	
	private ArmJoint[] Joints;

	private static readonly Vector3[] Axes = {Vector3.right,Vector3.up,Vector3.forward};

	public void Awake()
	{
		SetJointAngles();
	}

	public void SetJointAngles()
	{
		Joints = GetComponentsInChildren<ArmJoint>();
		for (int i=0; i<Joints.Length; i++)
		{
			float t = (float)i / (Joints.Length - 1);

			Joints[i].MinAngle = Mathf.Lerp(MinAngle.x, MinAngle.y, t);
			Joints[i].MaxAngle = Mathf.Lerp(MaxAngle.x, MaxAngle.y, t);

			Joints[i].Axis = Axes[i % Axes.Length];
		}
	}
}