using UnityEngine;
//using UModules;

public class RadialLookAt : MonoBehaviour
{
	const float SPEED = 2f*Mathf.PI;

	public float radius = 1f;
	public float zOffset = -100;

	float angle;
	Vector3 dir;
	Vector3 targetPos;
	Vector3 lookAtPos;
	Vector3 center;
	Camera mainCamera;

    public MenuController mc;

#if UNITY_EDITOR
	public bool debug;
#endif

	void Awake()
	{
		mainCamera = Camera.main;
		center = transform.localPosition;
	}

	void Update()
	{
		dir = Input.mousePosition - mainCamera.WorldToScreenPoint(transform.position);
        angle = Mathf.Atan2(dir.y, dir.x);
        targetPos = center + new Vector3(Mathf.Cos(angle),Mathf.Sin(angle)) * radius;
		transform.localPosition = targetPos;
		if(mc.invertedMovement)
        {
            lookAtPos = dir;
        }
        else
            lookAtPos = dir * -1;

        lookAtPos.z = center.z + zOffset;
		transform.localRotation = Quaternion.LookRotation(lookAtPos,Vector3.up);
	}

#if UNITY_EDITOR
	void OnDrawGizmos()
	{
		if (debug)
		{
			Gizmos.color = Color.magenta;
			Gizmos.DrawLine(center,targetPos);
		}
	}
#endif

}