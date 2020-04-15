using UnityEngine;

[System.Flags]
public enum Axis
{
	X = 0,
	Y = 1<<0,
	Z = 1<<1
}

public class ArmJoint : MonoBehaviour
{
	public float minAngle;
	public float maxAngle;
	public Axis axis;

	[UModules.Readonly]
	public Vector3 startOffset, zeroEuler;

	[Range(0f,1f)]
	public float slowdownThreshold = 0.5f;
	[Range(0f,360f)]
	// Degrees per second
	public float speed = 1f;

	void Awake ()
	{
		zeroEuler = transform.localEulerAngles;
		startOffset = transform.localPosition;
	}

	public float ClampAngle(float angle, float delta = 0f)
	{
		return Mathf.Clamp(angle + delta, minAngle, maxAngle);
	}

	public float GetAngle()
	{
		float angle = 0f;
		switch (axis)
		{
			case Axis.X:
			angle = transform.localEulerAngles.x;
			break;
			case Axis.Y:
			angle = transform.localEulerAngles.y;
			break;
			case Axis.Z:
			angle = transform.localEulerAngles.z;
			break;
		}
		return ClampAngle(angle);
	}

	public float SetAngle(float angle)
	{
		angle = ClampAngle(angle);
		switch (axis)
		{
			case Axis.X:
			transform.localEulerAngles = new Vector3(angle, 0, 0);
			break;
			case Axis.Y:
			transform.localEulerAngles = new Vector3(0, angle, 0);
			break;
			case Axis.Z:
			transform.localEulerAngles = new Vector3(0, 0, angle);
			break;
		}
		return angle;
	}

	public float MoveArm(float angle)
	{
		return SetAngle(angle);
	}

	#if UNITY_EDITOR
	private void OnDrawGizmos()
	{
		Debug.DrawLine(transform.position, transform.parent.position, Color.red);
	}
	#endif
}
