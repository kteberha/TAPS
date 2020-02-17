/*
	UModules::ColorAsset

	by: Peter Francis
	part of: UM_Basic

	available to use according to UM_Basic/LICENSE
*/

using UnityEngine;

namespace UModules
{
	/// <summary>
	/// Reusable asset to hold a single color;
	/// </summary>
	/// <module>UM_Basic</module>
	[CreateAssetMenu(fileName = "Color_", menuName = "UModules/Color", order = 0)]
	public class ColorAsset : ScriptableObject
	{
		/// <summary>
		/// The original stored value
		/// </summary>
		/// <access>private Color</access>
		[SerializeField] private Color value;
		/// <summary>
		/// The value to use after calling Set
		/// </summary>
		/// <access>private Color?</access>
		private Color? overrideValue = null;

		/// <summary>
		/// Override the value set in editor with a new value.
		/// (Setting value directly changes the serialized object, Set will only change the object's value for a single session.)
		/// </summary>
		/// <access>public Color</access>
		/// <param name="newValue" type="float">Override value to store</param>
		/// <returns>The new value</returns>
		public Color Set(Color newValue)
		{
			overrideValue = newValue;
			return newValue;
		}

		/// <summary>
		/// Implicitly convert a ColorAsset to a Color.
		/// Uses the override value created by Set if it exists, otherwise just the base stored value.
		/// </summary>
		/// <access>public static</access>
		/// <param name="colorAsset" type="ColorAsset">The asset to convert</param>
		public static implicit operator Color(ColorAsset colorAsset)
		{
			return colorAsset.overrideValue ?? colorAsset.value;
		}
	}
}
