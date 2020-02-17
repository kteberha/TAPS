using UnityEngine;

namespace UModules
{
	// TODO: No way to properly extend Physics?
	/// <summary>
	/// Extensions to Physics
	/// </summary>
	public static class PhysicsExtensions
	{
		public static bool RaycastTarget(this Physics physics, Vector3 origin, Vector3 direction, float maxDistance, GameObject target)
		{
			return Physics.Raycast(origin, direction, out RaycastHit hitInfo, maxDistance, 1<<target.layer)
				&& hitInfo.collider.gameObject == target;
		}

		public static bool SphereCastTarget(this Physics physics, Vector3 origin, float radius, Vector3 direction, float maxDistance, GameObject target)
		{
			return Physics.SphereCast(origin, radius, direction, out RaycastHit hitInfo, maxDistance, 1<<target.layer)
				&& hitInfo.collider.gameObject == target;
		}

		public static bool CheckPointTagged(Vector3 origin, int layerMask, QueryTriggerInteraction queryTriggerInteraction, string tag)
		{
			RaycastHit hit;
			return Physics.Raycast(origin, Vector3.up, out hit, 1f, layerMask, queryTriggerInteraction) && hit.collider.CompareTag(tag);
		}
	}
}