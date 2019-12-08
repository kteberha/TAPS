// www.ryanjuckett.com, Sunil Sandeep Nayak, Peter Francis
using UnityEngine;
using UModules;

[RequireComponent(typeof(Animator))]
public class ArmIK : MonoBehaviour
{

	[DontShowIf(nameof(mouseTarget))]
	public Transform target;
	public bool mouseTarget;
	[OnlyShowIf(nameof(mouseTarget))]
	public float screenToWorldPointFactor = 100f;
	[OnlyShowIf(nameof(mouseTarget))]
	public float zOffset = -10f;
	[SerializeField] private AvatarIKGoal Type;
	[SerializeField, Range(0, 1)] private float PositionWeight = 1;
	[SerializeField, Range(0, 1)] private float RotationWeight = 0;

	private Animator animator;
	private Vector3 targetPos;
	private Quaternion targetRot;
	private Camera mainCamera;

	void Awake()
	{
		animator = GetComponent<Animator>();
		mainCamera = Camera.main;
	}

	void OnAnimatorIK(int layerIndex)
	{
		if (mouseTarget)
		{
			//targetPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, transform.position.z - 10f);
			targetPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, mainCamera.nearClipPlane));
			targetPos *= screenToWorldPointFactor;
			targetPos.z = transform.position.z + zOffset;
			targetRot = Quaternion.identity;
		}
		else
		{
			targetPos = target.position;
			targetRot = target.rotation;
		}
		UpdateAnimatorIK(targetPos, targetRot);
	}

	private void UpdateAnimatorIK(Vector3 pos, Quaternion rot)
	{
		animator.SetIKPositionWeight(Type, PositionWeight);
		animator.SetIKRotationWeight(Type, RotationWeight);
		animator.SetIKPosition(Type, pos);
		animator.SetIKRotation(Type, rot);
	}
}

// OLD

// public class ArmIK : MonoBehaviour
// {
// 	const float ZETA = 0.1f;
// 	const float EPSILON = 0.001f;

// 	[SerializeField]
// 	private Transform[] joints;

// 	[SerializeField,DontShowIf(nameof(mouseTarget))]
// 	private Transform target;

// 	public bool mouseTarget;

// 	public bool onlyArmSide;

// 	[SerializeField,OnlyShowIf(nameof(onlyArmSide))]
// 	private ArmSide armSide;

// 	public bool debug;

// 	float[] theta;
// 	float[] sin;
// 	float[] cos;
// 	Vector3 targetPos;
// 	int tries;
// 	Transform rootJoint;
// 	Transform endJoint;
// 	PosRot[] jointsBase;
// 	bool resetLock;

// 	void Awake()
// 	{
// 		theta = new float[joints.Length];
// 		sin = new float[joints.Length];
// 		cos = new float[joints.Length];
// 		rootJoint = joints[0]; endJoint = joints[joints.Length-1];
// 		jointsBase = new PosRot[joints.Length];
// 		for (int i=0; i<joints.Length; i++)
// 		{
// 			jointsBase[i] = new PosRot(joints[i].position,joints[i].rotation);
// 		}
// 	}
	
// 	void LateUpdate()
// 	{
// 		//if (mouseTarget) {targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition); targetPos.z = transform.position.z;}
// 		if (mouseTarget) {targetPos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Camera.main.transform.position.z * -1));}
// 		else targetPos = target.transform.position;

// #if UNITY_EDITOR
// 		if (debug) Debug.LogFormat("ArmIK TargetPos: {0}", targetPos);
// #endif

// 		if (onlyArmSide)
// 		{
// 			switch (armSide)
// 			{
// 				case ArmSide.LEFT:
// 					if (targetPos.x < rootJoint.position.x) {Reset(); resetLock=true; return;}
// 				break;

// 				case ArmSide.RIGHT:
// 					if (targetPos.x > rootJoint.position.x) {Reset(); resetLock=true; return;}
// 				break;
				
// 				default:
// 					if (targetPos.x > rootJoint.position.x) {Reset(); resetLock=true; return;}
// 				break;
// 			}
// 		}

// 		resetLock = false;

//  		if (Vector3.Distance(endJoint.position,targetPos) > ZETA)
// 		{
// 				for (int i = joints.Length - 2; i >= 0; i--)
// 				{
// 					Vector3 r1 = joints[joints.Length - 1].transform.position - joints[i].transform.position;
// 					Vector3 r2 = targetPos - joints[i].transform.position;

// 					if (r1.magnitude * r2.magnitude <= EPSILON)
// 					{
// 						cos[i] = 1;
// 						sin[i] = 0;
// 					}
// 					else
// 					{
// 						cos[i] = Vector3.Dot(r1, r2) / (r1.magnitude * r2.magnitude);
// 						sin[i] = (Vector3.Cross(r1, r2)).magnitude / (r1.magnitude * r2.magnitude);
// 					}

// 					Vector3 axis = (Vector3.Cross(r1, r2)) / (r1.magnitude * r2.magnitude);
// 					theta[i] = Mathf.Acos(Mathf.Max(-1, Mathf.Min(1, cos[i])));

// 					if (sin[i] < 0.0f) theta[i] = -theta[i];
// 					theta[i] = (float)SimpleAngle(theta[i]) * Mathf.Rad2Deg;
// 					joints [i].transform.Rotate (axis, theta [i], Space.World);
// 				}
// 		}
// 	}

// 	private void Reset()
// 	{
// 		if (!resetLock)
// 		{
// #if UNITY_EDITOR
// 			if (debug) Debug.LogFormat("Reset Arm {0}", armSide);
// #endif
// 			for (int i=0; i<joints.Length; i++)
// 			{
// 				joints[i].SetPositionAndRotation(jointsBase[i].position,jointsBase[i].rotation);
// 			}
// 		}
// 	}

// 	private float SimpleAngle(float theta)
// 	{
// 		theta = theta % (2f * Mathf.PI);
// 		if (theta < -Mathf.PI)
// 			theta += 2f * Mathf.PI;
// 		else if (theta > Mathf.PI)
// 			theta -= 2f * Mathf.PI;
// 		return theta;
// 	}

// #if UNITY_EDITOR
// 	void OnDrawGizmos()
// 	{
// 		if (debug)
// 		{
// 			Gizmos.color = Color.green;
// 			for (int i=0; i<joints.Length-1; i++)
// 			{
// 				Gizmos.DrawLine(joints[i].position,joints[i+1].position);
// 			}
// 			Gizmos.color = Color.blue;
// 			Gizmos.DrawLine(joints[joints.Length-1].position,targetPos);
// 		}
// 	}
// #endif
// }

// // NOTE: Assuming X axis...
// public enum ArmSide {LEFT, RIGHT, NONE}

// struct PosRot
// {
// 	public Vector3 position;
// 	public Quaternion rotation;

// 	public PosRot(Vector3 pos, Quaternion rot)
// 	{
// 		this.position = pos;
// 		this.rotation = rot;
// 	}
// }
