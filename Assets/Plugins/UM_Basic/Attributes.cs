/*
	UModules::ButtonAttribute
	UModules::ReadonlyAttribute
	UModules::OnlyShowIfAttribute
	UModules::DontShowIfAttribute
	
	Future: UModules::OnlyShowWhenAttribute

	by: Rajin Shankar
	part of: UM_Basic

	available to use according to UM_Basic/LICENSE
*/

using UnityEngine;

namespace UModules
{
	/// <summary>
	/// Attribute to create a simple button that calls a method through SendMessage
	/// </summary>
	/// <module>UM_Basic</module>
	public class ButtonAttribute : PropertyAttribute
	{
		/// <summary>
		/// The name of the method to call
		/// </summary>
		/// <access>public string</access>
		public string methodName;
		/// <summary>
		/// The text to display on the button in the inspector
		/// </summary>
		/// <access>public string</access>
		public string displayName;
		/// <summary>
		/// Should the button be grouped on the same line as the property it's attached to?
		/// </summary>
		/// <access>public bool</access>
		public bool groupHorizontal;

		/// <summary>
		/// Should the property the button is attached to be displayed?
		/// </summary>
		public bool showProperty;

		public ButtonAttribute(string methodName, string displayName, bool groupHorizontal = true, bool showProperty = true)
		{
			this.methodName = methodName;
			this.displayName = displayName;
			this.groupHorizontal = groupHorizontal;
			this.showProperty = showProperty;
		}
		public ButtonAttribute(string methodName, bool groupHorizontal = true, bool showProperty = true) 
		: this(methodName, methodName, groupHorizontal, showProperty) { }

		/// <summary>
		/// Call SendMessage on a given target with the button's method name
		/// </summary>
		/// <param name="target" type="MonoBehaviour">The MonoBehaviour to send the message to</param>
		public void Invoke(MonoBehaviour target)
		{
			target.SendMessage(methodName, SendMessageOptions.DontRequireReceiver);
		}
	}

	// Restrict to methods only
	[System.AttributeUsage(System.AttributeTargets.Method)]
	public class ExposeMethodAttribute : System.Attribute
	{
		public bool RuntimeOnly = true;
	}

	/// <summary>
	/// Attribute to display a property but disable editing
	/// </summary>
	/// <module>UM_Basic</module>
	/// <example>
	/// using UModules;
	/// public class Foo : ExtendedBehaviour
	/// {
	///     public float editableValue;
	/// 
	///     [Readonly]
	///     public float uneditableValue;
	/// }
	/// </example>
	public class ReadonlyAttribute : PropertyAttribute { }

	/// <summary>
	/// Attribute to only display a property when a given serialized property is true
	/// </summary>
	/// <module>UM_Basic</module>
	/// <example>
	/// using UModules;
	/// public class Foo : ExtendedBehaviour
	/// {
	///     [SerializeField]
	///     private bool showAdditionalValues = false;
	/// 
	///     [OnlyShowIf("showAdditionalValues")]
	///     public float value1;
	/// 
	///     [OnlyShowIf("showAdditionalValues")]
	///     public int value2, value3, value4;
	/// }
	/// </example>
	public class OnlyShowIfAttribute : PropertyAttribute
	{
		/// <summary>
		/// The name of the serialized property to check
		/// </summary>
		/// <access>public string</access>
		public string propertyName;

		public OnlyShowIfAttribute(string propertyName)
		{
			this.propertyName = propertyName;
		}
	}
	/// <summary>
	/// Attribute to only display a property when a given serialized property is false
	/// </summary>
	/// <module>UM_Basic</module>
	/// <example>
	/// using UModules;
	/// public class Foo : ExtendedBehaviour
	/// {
	///     [SerializeField]
	///     private bool useSimpleMode = true;
	/// 
	///     [DontShowIf("useSimpleMode")]
	///     public float value1;
	/// 
	///     [DontShowIf("useSimpleMode")]
	///     public int value2, value3, value4;
	/// }
	/// </example>
	public class DontShowIfAttribute : PropertyAttribute
	{
		/// <summary>
		/// The name of the serialized property to check
		/// </summary>
		/// <access>public string</access>
		public string propertyName;

		public DontShowIfAttribute(string propertyName)
		{
			this.propertyName = propertyName;
		}
	}

	public class TagSelectorAttribute : PropertyAttribute {}

	// Note: Lambdas in attributes not supported in C# (yet)
	// /// <summary>
	// /// Attribute to only diplay a property when a given predicate is true
	// /// </summary>
	// /// <module>UM_Basic</module>
	// public class OnlyShowWhenAttribute : PropertyAttribute
	// {
	//     public System.Func<bool> predicate;

	//     public OnlyShowWhenAttribute(System.Func<bool> predicate)
	//     {
	//         this.predicate = predicate;
	//     }
	// }

 	[System.AttributeUsage(System.AttributeTargets.Field)]
    public class ReorderableListAttribute : PropertyAttribute
    {
        public float r, g, b;

        public bool disableAdding;

        public bool disableRemoving;

        public bool disableAddingAndRemoving
        {
            get { return disableAdding && disableRemoving; }
            set { disableAdding = disableRemoving = value; }
        }

        public bool disableDragging;

        public bool elementsAreSubassets;

        public string elementHeaderFormat;

        public string listHeaderFormat;

        public bool hideFooterButtons;

        public string[] parallelListNames;

        public enum ParallelListLayout { Rows, Columns };

        public ParallelListLayout parallelListLayout;

        public ReorderableListAttribute() { }

        public ReorderableListAttribute(params string[] parallelListNames)
        {
            this.parallelListNames = parallelListNames;
        }

        public const string SingularPluralBlockBegin = "{{";
        public const string SingularPluralBlockSeparator = "|";
        public const string SingularPluralBlockEnd = "}}";

        public string singularListHeaderFormat
        {
            get
            {
                if (listHeaderFormat == null)
                    return null;
                var value = listHeaderFormat;
                while (value.Contains(SingularPluralBlockBegin)) {
                    int beg = value.IndexOf(SingularPluralBlockBegin);
                    int end = value.IndexOf(SingularPluralBlockEnd, beg);
                    if (end < 0) break;
                    end += SingularPluralBlockEnd.Length;
                    int blockLen = end - beg;
                    var block = value.Substring(beg, blockLen);
                    int sep = value.IndexOf(SingularPluralBlockSeparator, beg);
                    if (sep < 0) {
                        value = value.Replace(block, "");
                    }
                    else
                    {
                        beg += SingularPluralBlockBegin.Length;
                        int singularLen = (sep - beg);
                        var singular = value.Substring(beg, singularLen);
                        value = value.Replace(block, singular);
                    }
                }
                return value;
            }
        }

        public string pluralListHeaderFormat
        {
            get
            {
                if (listHeaderFormat == null)
                    return null;
                var value = listHeaderFormat;
                while (value.Contains(SingularPluralBlockBegin)) {
                    int beg = value.IndexOf(SingularPluralBlockBegin);
                    int end = value.IndexOf(SingularPluralBlockEnd, beg);
                    if (end < 0) break;
                    end += SingularPluralBlockEnd.Length;
                    int blockLen = end - beg;
                    var block = value.Substring(beg, blockLen);
                    int sep = value.IndexOf(SingularPluralBlockSeparator, beg);
                    if (sep < 0) {
                        beg += SingularPluralBlockBegin.Length;
                        end -= SingularPluralBlockEnd.Length;
                        int pluralLen = (end - beg);
                        var plural = value.Substring(beg, pluralLen);
                        value = value.Replace(block, plural);
                    }
                    else
                    {
                        sep = sep + SingularPluralBlockSeparator.Length;
                        end -= SingularPluralBlockEnd.Length;
                        int pluralLen = (end - sep);
                        var plural = value.Substring(beg, pluralLen);
                        value = value.Replace(block, plural);
                    }
                }
                return value;
            }
        }

    }

	public class EnumFlagAttribute : PropertyAttribute
	{
		public string name;

		public EnumFlagAttribute() { }

		public EnumFlagAttribute(string name)
		{
			this.name = name;
		}
	}
}