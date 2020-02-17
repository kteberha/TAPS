/*
	UModules::Tween

	by: Peter Francis
	part of: UM_Basic

	available to use according to UM_Basic/LICENSE
*/

using System;
using System.Collections;
using UnityEngine;

namespace UModules.Tweening
{
	public static class Tween
	{
		/// <summary>
		/// Evaluate the curve at the given time with respect to the given scale and duration
		/// Time is clamped between 0 and duration
		/// </summary>
		private static float Evaluate(EaseEquations.EaseDelegate ease, float t, float v, float duration, float scale)
		{
			// Clamp time value in the range [0, duration]
			if (t < 0) t = 0;
			else if (t > duration) t = duration;
			// Evaluate curve in range [0, 1] with respect to given duration
			return ease(t / duration, v, 1f-v, duration) * scale;
		}

		/// <summary>
		/// Evaluate the curve at the given time with respect to the given scale and duration
		/// Time is not clamped between 0 and duration
		/// </summary>
		private static float EvaluateUnclamped(EaseEquations.EaseDelegate ease, float t, float v, float duration, float scale)
		{
			// Evaluate curve in range [0, 1] with respect to given duration
			return ease(t / duration, v, 1f-v, duration) * scale;
		}

		/// <summary>
		/// Generate a coroutine that can be run on a MonoBehaviour to evaluate a callback with the curve's value over time, with added actions called before and after evaluation
		/// </summary>
		/// <access>public IEnumerator</access>
		/// <param name="ease" type="Ease">Ease function</param>
		/// <param name="duration" type="float">Inverse scale factor for input time</param>
		/// <param name="scale" type="float">Scale factor for curve value</param>
		/// <param name="callback" type="Action(float)">Callback action that takes an input float value</param>
		/// <param name="onStart" type="Action()">Action run before curve evaluation begins</param>
		/// <param name="onEnd" type="Action()">Action run after curve evaluation finishes</param>
		/// <returns>The generated routine</returns>
		public static IEnumerator Run(Easing ease, float duration, float scale, Action<float> callback, Action onStart, Action onEnd)
		{
			// Call start action
			if (onStart != null) onStart();
			float t = 0;
			float v = 0;
			EaseEquations.EaseDelegate easeFunction = EaseEquations.methods[(int)ease];
			while (t < duration)
			{
				// Clamp input time
				t += Time.deltaTime;
				if (t > duration) t = duration;
				// Call callback action with curve value at current time
				v=EvaluateUnclamped(easeFunction, t, v, duration, scale);
				callback(v);
				// Wait for next frame
				yield return null;
			}
			// Call end action
			if (onEnd != null) onEnd();
		}
		/// <summary>
		/// Generate a coroutine that can be run on a MonoBehaviour to evaluate a callback with the curve's value over time
		/// </summary>
		/// <param name="ease" type="Ease">Ease function</param>
		/// <param name="duration" type="float">Inverse scale factor for input time</param>
		/// <param name="scale" type="float">Scale factor for curve value</param>
		/// <param name="callback" type="Action(float)">Callback action that takes an input float value</param>
		/// <returns>The generated routine</returns>
		/// <seealso cref="Tween.Run(Easing, float, float, Action{float}, Action, Action)" />
		public static IEnumerator Run(Easing ease, float duration, float scale, Action<float> callback) { return Run(ease, duration, scale, callback, null, null); }

		/// <summary>
		/// Extension method to start a curve's Run routine on a given MonoBehaviour
		/// </summary>
		/// <access>public static void</access>
		/// <param name="behaviour" type="this MonoBehaviour">The behaviour to start the routine on</param>
		/// <param name="curve" type="CurveAsset">The curve to get the routine from</param>
		/// <param name="duration" type="float">Inverse scale factor for input time</param>
		/// <param name="scale" type="float">Scale factor for curve value</param>
		/// <param name="callback" type="Action(float)">Callback action that takes an input float value</param>
		/// <param name="onStart" type="Action()">Action run before curve evaluation begins</param>
		/// <param name="onEnd" type="Action()">Action run after curve evaluation finishes</param>
		/// <seealso cref="Tween.Run(Easing, float, float, Action{float}, Action, Action)" />
		public static void RunTween(this MonoBehaviour behaviour, Easing ease, float duration, float scale, Action<float> callback, Action onStart, Action onEnd)
		{
			behaviour.StartCoroutine(Run(ease, duration, scale, callback, onStart, onEnd));
		}
		/// <summary>
		/// Extension method to start a curve's Run routine on a given MonoBehaviour
		/// </summary>
		/// <access>public static void</access>
		/// <param name="behaviour" type="this MonoBehaviour">The behaviour to start the routine on</param>
		/// <param name="curve" type="CurveAsset">The curve to get the routine from</param>
		/// <param name="duration" type="float">Inverse scale factor for input time</param>
		/// <param name="scale" type="float">Scale factor for curve value</param>
		/// <param name="callback" type="Action(float)">Callback action that takes an input float value</param>
		public static void RunTween(this MonoBehaviour behaviour, Easing ease, float duration, float scale, Action<float> callback)
		{
			behaviour.StartCoroutine(Run(ease, duration, scale, callback));
		}

		/// <summary>
		/// Extension method to start a curve's Run routine on a given MonoBehaviour if it is not already active
		/// isActive is set true before calling onStart and set false after calling onEnd
		/// </summary>
		/// <access>public static void</access>
		/// <param name="behaviour" type="this MonoBehaviour">The behaviour to start the routine on</param>
		/// <param name="curve" type="CurveAsset">The curve to get the routine from</param>
		/// <param name="duration" type="float">Inverse scale factor for input time</param>
		/// <param name="scale" type="float">Scale factor for curve value</param>
		/// <param name="callback" type="Action(float)">Callback action that takes an input float value</param>
		/// <param name="onStart" type="Action()">Action run before curve evaluation begins</param>
		/// <param name="onEnd" type="Action()">Action run after curve evaluation finishes</param>
		/// <param name="isActive" type="Reference<bool>">Is the curve already being run?</param>
		public static void RunTween(this MonoBehaviour behaviour, Easing ease, float duration, float scale, Action<float> callback, Action onStart, Action onEnd, Reference<bool> isActive)
		{
			if (isActive) return;
			behaviour.StartCoroutine(Run(ease, duration, scale, callback, () => { isActive.Set(true); onStart(); }, () => { onEnd(); isActive.Set(false); }));
		}
	}
}