using UnityEngine;

public class ArmJoint : MonoBehaviour
{

	public float MinAngle;
	public float MaxAngle;
	public Vector3 Axis;

	[HideInInspector]
	public Vector3 StartOffset, ZeroEuler;

	[Range(0, 1f)]
	public float SlowdownThreshold = 0.5F;

	[Range(0, 360f)]
	// Degrees per second
	public float Speed = 1F;

	void Awake ()
	{
		StartOffset = transform.localPosition;
	}

	public float ClampAngle(float angle, float delta = 0)
	{
		return Mathf.Clamp(angle + delta, MinAngle, MaxAngle);
	}

	public float GetAngle()
	{
		float angle = 0;
		if (Axis.x == 1) angle = transform.localEulerAngles.x;
		else
		if (Axis.y == 1) angle = transform.localEulerAngles.y;
		else
		if (Axis.z == 1) angle = transform.localEulerAngles.z;
		return ClampAngle(angle);
	}

	public float SetAngle(float angle)
	{
		angle = ClampAngle(angle);
		if (Axis.x == 1) transform.localEulerAngles = new Vector3(angle, 0, 0);
		else
		if (Axis.y == 1) transform.localEulerAngles = new Vector3(0, angle, 0);
		else
		if (Axis.z == 1) transform.localEulerAngles = new Vector3(0, 0, angle);
		return angle;
	}

	public float MoveArm(float angle)
	{
		return SetAngle(angle);
	}
}
