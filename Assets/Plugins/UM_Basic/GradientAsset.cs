/*
	UModules::GradientAsset

	by: Peter Francis
	part of: UM_Basic

	available to use according to UM_Basic/LICENSE
*/

using UnityEngine;

namespace UModules
{
	/// <summary>
	/// Reusable asset to hold a single gradient;
	/// </summary>
	/// <module>UM_Basic</module>
	[CreateAssetMenu(fileName = "Gradient_", menuName = "UModules/Gradient", order = 0)]
	public class GradientAsset : ScriptableObject
	{
		/// <summary>
		/// The original stored value
		/// </summary>
		/// <access>private Gradient</access>
		[SerializeField] private Gradient value;

		// TODO: Gradient is not #nullable annotated
		/// <summary>
		/// The value to use after calling Set
		/// </summary>
		/// <access>private Gradient?</access>
		private Gradient overrideValue = null;

		/// <summary>
		/// Override the value set in editor with a new value.
		/// (Setting value directly changes the serialized object, Set will only change the object's value for a single session.)
		/// </summary>
		/// <access>public Gradient</access>
		/// <param name="newValue" type="float">Override value to store</param>
		/// <returns>The new value</returns>
		public Gradient Set(Gradient newValue)
		{
			overrideValue = newValue;
			return newValue;
		}

		public Gradient Gradient {get {return value;}}

		// /// <summary>
		// /// Implicitly convert a GradientAsset to a Gradient.
		// /// Uses the override value created by Set if it exists, otherwise just the base stored value.
		// /// </summary>
		// /// <access>public static</access>
		// /// <param name="GradientAsset" type="GradientAsset">The asset to convert</param>
		// public static implicit operator Gradient(GradientAsset GradientAsset)
		// {
		// 	return GradientAsset.overrideValue ?? GradientAsset.value;
		// }
	}
}
