using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RaySensor
{
	[Range(0f,1000f)]
	public float length = 5f;
	public Vector2 direction = Vector2.zero;
	public LayerMask mask;
	public Space spaceSetting;
	// RaycastHit2D[] hits;

	public bool Test(Transform t, out RaycastHit2D hit)
	{
		Vector2 origin = t.position;
		Vector2 dir = spaceSetting==Space.Self?t.InverseTransformDirection(direction):t.TransformDirection(direction);
		hit = Physics2D.Raycast(origin,dir,length,mask);
		return hit.collider != null;
	}

	class RayDistanceComparer2D : IComparer<RaycastHit2D>
	{
		public int Compare(RaycastHit2D x, RaycastHit2D y)
		{
			if (x.distance < y.distance) { return -1; }
			else if (x.distance > y.distance) { return 1; }
			else { return 0; }
		}
	}
}