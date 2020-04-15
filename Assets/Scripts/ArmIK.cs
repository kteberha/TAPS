// www.ryanjuckett.com, Sunil Sandeep Nayak, Peter Francis
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ArmIK : MonoBehaviour
{

	public Transform target;
	public bool mouseTarget;
	public float screenToWorldPointFactor = 100f;
	public float zOffset = -10f;
	[SerializeField]
	private AvatarIKGoal Type;
	[SerializeField, Range(0, 1)]
	private float PositionWeight = 1;
	[SerializeField, Range(0, 1)]
	private float RotationWeight = 0;

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

