using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Random=System.Random;

namespace UModules
{
	/// <summary>
	/// Miscellaneous Extensions
	/// </summary>
	public static class MiscExtensions
	{
		public static void RemoveComponent<T>(this GameObject go, bool warnIfNotFound = true, bool immediate = false) where T : Component
		{
			T target = go.GetComponent<T>();
			if (warnIfNotFound && target == null) { Debug.LogWarningFormat("Could not get component {0} to remove!", typeof(T).ToString()); }
			if (immediate)
			{
				UnityEngine.Object.DestroyImmediate(target);
			}
			else
			{
				UnityEngine.Object.Destroy(target);
			}
		}

		public static void RemoveComponent(this GameObject go, System.Type componentType, bool warnIfNotFound = true, bool immediate = false)
		{
			Component target = go.GetComponent(componentType);
			if (warnIfNotFound && target == null) { Debug.LogWarningFormat("Could not get component {0} to remove!", componentType.ToString()); }
			if (immediate)
			{
				UnityEngine.Object.DestroyImmediate(target);
			}
			else
			{
				UnityEngine.Object.Destroy(target);
			}
		}

		public static GameObject AddChildGameObject(this GameObject go, string name = "NewChild", params Type[] componentTypes)
		{
			GameObject newChild = new GameObject(name,componentTypes);
			newChild.transform.SetParent(go.transform);
			return newChild;
		}

		public static GameObject AddChildGameObject(this GameObject go, GameObject newGo)
		{
			newGo.transform.SetParent(go.transform);
			return newGo;
		}

		public static GameObject FindChild(this GameObject go, string name)
		{
			Transform childT = go.transform.Find(name);
			return childT != null ? childT.gameObject : null;
		}

		public static IEnumerable<GameObject> GetChildren(this GameObject go)
		{
			return go.transform.GetEnumerator().Iterate().Cast<Transform>().Select((x)=>x.gameObject);
		}

		public static IEnumerable Iterate(this IEnumerator iterator)
		{
			while(iterator.MoveNext())
			{
				yield return iterator.Current;
			}
		}

		public static void LogAll(this IEnumerable list)
		{
			foreach (object item in list)
			{
				Debug.Log(item);
			}
		}

		public static Dictionary<string,float> GetClipTimes(this Animator animator)
		{
			Dictionary<string,float> clips = new Dictionary<string, float>();
			foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
			{
				clips.Add(clip.name,clip.length);
			}
			return clips;
		}

		public static float GetClipTime(this Animator animator, string name)
		{
			return animator.runtimeAnimatorController.animationClips.SingleOrDefault((x)=>x.name==name).length;
		}

		public static IEnumerable<Type> GetDerivedTypes(this Type baseType)
		{
			return from Type t in Assembly.GetAssembly(baseType).GetTypes() where t.IsSubclassOf(baseType) select t;
			//return Assembly.GetExecutingAssembly().GetTypes().Where(t => baseType.IsSubclassOf(t));
		}


		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source)
		{
			return source.Shuffle(new Random());
		}

		public static IEnumerable<T> Shuffle<T>(this IEnumerable<T> source, Random rng)
		{
			if (source == null) throw new ArgumentNullException("source");
			if (rng == null) throw new ArgumentNullException("rng");

			return source.ShuffleIterator(rng);
		}

		private static IEnumerable<T> ShuffleIterator<T>(
			this IEnumerable<T> source, Random rng)
		{
			var buffer = source.ToList();
			for (int i = 0; i < buffer.Count; i++)
			{
				int j = rng.Next(i, buffer.Count);
				yield return buffer[j];

				buffer[j] = buffer[i];
			}
		}


		public static List<MethodInfo> GetMethods(this Type type, Type returnType, Type[] paramTypes, BindingFlags flags)
		{
			return type.GetType().GetMethods(flags)
				.Where(m => m.ReturnType == returnType)
				.Select(m => new { m, Params = m.GetParameters() })
				.Where(x =>
					{
						return paramTypes == null ? // no params?
							x.Params.Length == 0 :
							x.Params.Length == paramTypes.Length &&
							x.Params.Select(p => p.ParameterType).SequenceEqual(paramTypes);
					})
				.Select(x => x.m)
				.ToList();
		}		

		public static List<MethodInfo> GetMethods(this MonoBehaviour mb, Type returnType, Type[] paramTypes, BindingFlags flags)
		{
			return mb.GetType().GetMethods(flags)
				.Where(m => m.ReturnType == returnType)
				.Select(m => new { m, Params = m.GetParameters() })
				.Where(x =>
					{
						return paramTypes == null ? // no params?
							x.Params.Length == 0 :
							x.Params.Length == paramTypes.Length &&
							x.Params.Select(p => p.ParameterType).SequenceEqual(paramTypes);
					})
				.Select(x => x.m)
				.ToList();
		}
		
		public static List<MethodInfo> GetMethods(this GameObject go, Type returnType, Type[] paramTypes, BindingFlags flags)
		{
			MonoBehaviour[] mbList = go.GetComponents<MonoBehaviour>();
			List<MethodInfo> list = new List<MethodInfo>();
			foreach (MonoBehaviour mb in mbList) {
				list.AddRange(mb.GetMethods(returnType, paramTypes, flags));
			}
			return list;
		}

		public static T[] GetInArray<T>(this T[,] matrix, int index, bool horizontal=true)
		{
			T[] arr = new T[(horizontal?matrix.GetLength(0):matrix.GetLength(1))];
			for (int i=0; i<(horizontal?matrix.GetLength(0):matrix.GetLength(1)); i++)
			{
				if (horizontal)
				{
					arr[i] = matrix[index,i];
				}
				else
				{
					arr[i] = matrix[i,index];
				}
			}
			return arr;
		}

		public static void ActInArray<T>(this T[,] matrix, int index, Action<T> act, bool horizontal=true)
		{
			for (int i=0; i<(horizontal?matrix.GetLength(0):matrix.GetLength(1)); i++)
			{
				if (horizontal)
				{
					act(matrix[index,i]);
				}
				else
				{
					act(matrix[i,index]);
				}
			}
		}

		public static void SetInArray<T>(this T[,] matrix, int index, Func<T,T> func, bool horizontal=true)
		{
			for (int i=0; i<(horizontal?matrix.GetLength(0):matrix.GetLength(1)); i++)
			{
				if (horizontal)
				{
					matrix[index,i] = func(matrix[index,i]);
				}
				else
				{
					matrix[i,index] = func(matrix[i,index]);
				}
			}
		}

		public static void SetInArray<T>(this T[,] matrix, int index, Func<T,T> func, T[] tm, bool horizontal=true)
		{
			for (int i=0; i<tm.Length; i++)
			{
				if (horizontal)
				{
					matrix[index,i] = func(tm[i]);
				}
				else
				{
					matrix[i,index] = func(tm[i]);
				}
			}
		}

		public static void ShiftInArray<T>(this T[,] matrix, int index, int dir)
		{
			if (index >= matrix.GetLength(0) || index >= matrix.GetLength(1) || index < 0) {
				return;
			}
			switch (dir)
			{
				case -2:
				{
					if (index == 0) {
						return;
					}
					for (int i=0; i<matrix.GetLength(1); i++) {
						matrix[i,index] = matrix[i,index-1];
					}
					return;
				}
				case 2: {
					if (index >= matrix.GetLength(1)-1) {
						return;
					}
					for (int i=0; i<matrix.GetLength(1); i++) {
						matrix[i,index] = matrix[i,index+1];
					}
					return;
				}
				case -1: {
					if (index == 0) {
						return;
					}
					for (int i=0; i<matrix.GetLength(0); i++) {
						matrix[index,i] = matrix[index-1,i];
					}
					return;
				}
				case 1: {
					if (index >= matrix.GetLength(0)-1) {
						return;
					}
					for (int i=0; i<matrix.GetLength(0); i++) {
						matrix[index,i] = matrix[index+1,i];
					}
					return;
				}
			}
		}
	}
}