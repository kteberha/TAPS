using UnityEngine;
using UModules;

public class SetJointWeights : MonoBehaviour
{
	public ArmJoint rootJoint;

	public Vector2 minAngle;
	public Vector2 maxAngle;

	public float minSpeed;
	public float maxSpeed;
	
	public Axis[] axes = {Axis.Z};

	private ArmJoint[] joints;

	public void Awake()
	{
		joints = rootJoint?.GetComponentsInChildren<ArmJoint>()??GetComponentsInChildren<ArmJoint>();
		SetJointAngles();
	}

	[ExposeMethod(RuntimeOnly=false)]
	public void SetJointAngles()
	{
		joints = rootJoint?.GetComponentsInChildren<ArmJoint>()??GetComponentsInChildren<ArmJoint>();
		for (int i=0; i<joints.Length; i++)
		{
			float t = (float)i / (joints.Length - 1);

			joints[i].minAngle = Mathf.Lerp(minAngle.x, minAngle.y, t);
			joints[i].maxAngle = Mathf.Lerp(maxAngle.x, maxAngle.y, t);

			joints[i].speed = Mathf.Lerp(minSpeed, maxSpeed, t);

			joints[i].axis = axes[i % axes.Length];
		}
	}
}