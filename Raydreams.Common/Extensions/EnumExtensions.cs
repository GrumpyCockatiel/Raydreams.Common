using System;
using System.ComponentModel;
using System.Reflection;

namespace Raydreams.Common.Extensions
{
	/// <summary></summary>
	public static class EnumExtensions
	{
		/// <summary>Gets the <see cref="DescriptionAttribute"/> of the value, otherwise returns the string value of the value</summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static string GetDescription( this Enum value )
		{
			FieldInfo fieldInfo = value.GetType().GetField( value.ToString() );

			DescriptionAttribute descriptionAttribute = Attribute.GetCustomAttribute( fieldInfo, typeof( DescriptionAttribute ) ) as DescriptionAttribute;

			return descriptionAttribute == null ? value.ToString() : descriptionAttribute.Description;
		}
	}
}
