using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace UModules
{
	/// <summary>
	/// Extensions to Vector types
	/// </summary>
	public static class VectorExtensions
	{
		/// <summary>
		/// Return the result of scaling a vector along the x or y axes
		/// </summary>
		/// <access>public static Vector2</access>
		/// <param name="v" type="this Vector2">Input vector to scale</param>
		/// <param name="sx" type="float">X scale factor</param>
		/// <param name="sy" type="float">Y scale factor</param>
		/// <returns>A new Vector2 (v.x * sx, v.y * sy)</returns>
		public static Vector2 Scale(this Vector2 v, float sx = 1, float sy = 1)
		{
			return new Vector2(v.x * sx, v.y * sy);
		}

		/// <summary>
		/// Return the result of scaling a vector along the x and y axes
		/// </summary>
		/// <access>public static Vector2</access>
		/// <param name="A" type="this Vector2">Input vector to scale</param>
		/// <param name="B" type="Vector2">Scale vector</param>
		/// <returns>A new Vector2 (A.x * B.x, A.y * B.y)</returns>
		public static Vector2 Multiply(this Vector2 A, Vector2 B)
		{
			return new Vector2(A.x * B.x, A.y * B.y);
		}

		/// <summary>
		/// Return the result of dividing a vector along the x and y axes
		/// </summary>
		/// <param name="A" type="this Vector2">Input vector to divide</param>
		/// <param name="B" type="Vector2">Dividing vector</param>
		/// <returns>A new Vector2 (A.x / B.x, A.y / B.y)</returns>
		public static Vector2 Divide(this Vector2 A, Vector2 B)
		{
			return new Vector2(A.x / B.x, A.y / B.y);
		}

		/// <summary>
		/// Return the result of scaling a vector along the x, y, or z axes
		/// </summary>
		/// <access>public static Vector3</access>
		/// <param name="v" type="this Vector3">Input vector to scale</param>
		/// <param name="sx" type="float">X scale factor</param>
		/// <param name="sy" type="float">Y scale factor</param>
		/// <param name="sz" type="float">Z scale factor</param>
		/// <returns>A new Vector3 (v.x * sx, v.y * sy, v.z * sz)</returns>
		public static Vector3 Scale(this Vector3 v, float sx = 1, float sy = 1, float sz = 1)
		{
			return new Vector3(v.x * sx, v.y * sy, v.z * sz);
		}

		/// <summary>
		/// Return the result of scaling a vector along the x, y, and z axes
		/// </summary>
		/// <param name="A" type="this Vector3">Input vector to scale</param>
		/// <param name="B" type="Vector3">Scale vector</param>
		/// <returns>A new Vector3 (A.x * B.x, A.y * B.y, A.z * B.z)</returns>
		public static Vector3 Multiply(this Vector3 A, Vector3 B)
		{
			return new Vector3(A.x * B.x, A.y * B.y, A.z * B.z);
		}

		/// <summary>
		/// Return the result of dividing a vector along the x, y, and z axes
		/// </summary>
		/// <param name="A" type="this Vector3">Input vector to divide</param>
		/// <param name="B" type="Vector3">Dividing vector</param>
		/// <returns>A new Vector3 (A.x / B.x, A.y / B.y, A.z / B.z)</returns>
		public static Vector3 Divide(this Vector3 A, Vector3 B)
		{
			return new Vector3(A.x / B.x, A.y / B.y, A.z / B.z);
		}

		/// <summary>
		/// Return the result of making a new Vector2 from x and z components of the given Vector3
		/// </summary>
		/// <param name="v">Input vector</param>
		/// <returns>A new Vector2 (v.x, v.z)</returns>
		public static Vector2 ToVec2(this Vector3 v)
		{
			return new Vector2(v.x, v.z);
		}

		/// <summary>
		/// Return the result of making a new Vector3 from x and y components of the given Vector2
		/// </summary>
		/// <param name="v">Input vector</param>
		/// <returns>A new Vector3 (v.x, 0, v.y)</returns>
		public static Vector3 ToVec3(this Vector2 v)
		{
			return new Vector3(v.x, 0f, v.y);
		}

		// Return a random vector on the unit circle by generating an angle and taking the sine and cosine
		public static Vector2 RandomOnUnitCircle()
		{
			float th = UnityEngine.Random.value * 360 * Mathf.Deg2Rad;
			return new Vector2(Mathf.Cos(th), Mathf.Sin(th));
		}

		public static Vector3 RandomInRadius(this Vector3 center, float radius)
		{
			float a = UnityEngine.Random.value * 2f * Mathf.PI;
			float r = radius * Mathf.Sqrt(UnityEngine.Random.value);
			return new Vector3(center.x+r*Mathf.Cos(a), center.y, center.z+r*Mathf.Sin(a));
		}

		public static Vector3 RandomInRadius(this Vector3 center, float radius, int seed)
		{
			UnityEngine.Random.InitState(seed);
			float a = UnityEngine.Random.value * 2f * Mathf.PI;
			float r = radius * Mathf.Sqrt(UnityEngine.Random.value);
			return new Vector3(center.x+r*Mathf.Cos(a), center.y, center.z+r*Mathf.Sin(a));
		}

		// https://stackoverflow.com/questions/13064912/generate-a-uniformly-random-point-within-an-annulus-ring
		/// <summary>
		/// Generate a random point on an annulus given the center, outer radius r1, and inner radius r2 where r1>=r2
		/// </summary>
		/// <param name="center">Center point</param>
		/// <param name="r1" type="float">Outer radius</param>
		/// <param name="r2">Inner radius</param>
		/// <returns>A point outside r1 and inside r2; new Vector2(?, ?)</returns>
		public static Vector2 RandomOnAnnulus(this Vector2 center, float r1, float r2)
		{
			float t = (2f*Mathf.PI*Random.value)*Mathf.Deg2Rad;
			float u = Random.value+Random.value;
			//float u = Random.Range(r2,r1);
			//float v = Random.Range(0f,r2+r1);
			float r = u>1 ? 2-u : u;
			r = r<r2 ? r2+r*((r1-r2)/r2) : r;
			//float r = v<u ? u : r2+r1-u;
			return new Vector2(center.x+r*Mathf.Cos(t),center.y+r*Mathf.Sin(t));
		}

		/// <summary>
		/// Generate a random point on an annulus given the center, outer radius r1, and inner radius r2 where r1>=r2,
		/// retaining the y component of the center
		/// </summary>
		/// <param name="center">Center point</param>
		/// <param name="r1" type="float">Outer radius</param>
		/// <param name="r2">Inner radius</param>
		/// <returns>A point outside r1 and inside r2; new Vector3(?, center.y, ?)</returns>
		public static Vector3 RandomOnAnnulus(this Vector3 center, float r1, float r2)
		{
			float t = (2f*Mathf.PI*Random.value)*Mathf.Deg2Rad;
			float u = Random.value+Random.value;
			//float u = Random.Range(r2,r1);
			//float v = Random.Range(0f,r2+r1);
			float r = u>1 ? 2-u : u;
			r = r<r2 ? r2+r*((r1-r2)/r2) : r;
			//float r = v<u ? u : r2+r1-u;
			return new Vector3(center.x+r*Mathf.Cos(t),center.y,center.z+r*Mathf.Sin(t));
		}

		public static Vector3 RandomInBox(this Vector3 center, Vector3 size)
		{
			return center + new Vector3(
				(UnityEngine.Random.value-0.5f)*size.x,
				(UnityEngine.Random.value-0.5f)*size.y,
				(UnityEngine.Random.value-0.5f)*size.z);
		}

		public static Vector3 RandomInBox(this Vector3 center, Vector3 size, int seed)
		{
			UnityEngine.Random.InitState(seed);
			return center + new Vector3(
				(UnityEngine.Random.value-0.5f)*size.x,
				(UnityEngine.Random.value-0.5f)*size.y,
				(UnityEngine.Random.value-0.5f)*size.z);
		}

		public static Vector3 Opposite(this Vector3 v1, Vector3 v2)
		{
			return -((v2-v1).normalized);
		}

		public static bool Within(this Vector3 v1, Vector3 v2, float distance)
		{
			return Vector3.Distance(v1,v2) <= distance;
		}

		public static Vector3 Mean(this Vector3 first, params Vector3[] others)
		{
			if (others.Length==0) return first;
			return others.Aggregate(first,(x,y)=>x+y)/(others.Length+1);
		}

		//public static Vector3 Median(this Vector3 first, params Vector3[] others)

		public static float SignedAngleXY(this Vector2 a, Vector2 b)
		{
			var aa = Mathf.Atan2(a.x, a.y) * Mathf.Rad2Deg;
			var ba = Mathf.Atan2(b.x, b.y) * Mathf.Rad2Deg;
			return Mathf.DeltaAngle(aa, ba);
		}

		public static float SignedAngleXY(this Vector3 a, Vector3 b)
		{
			return SignedAngleXY((Vector2)a,(Vector2)b);
		}
	}
}