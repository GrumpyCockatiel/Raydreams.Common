using System;
using System.Reflection;

namespace Raydreams.Common.Validation
{
	/// <summary>A very simple object validator</summary>
	/// <typeparam name="T"></typeparam>
	public class SimpleValidator<T>
	{
		public SimpleValidator()
		{
		}

		/// <summary>Simply iterates over an instance and if the property is a string checks for Null or White Space only</summary>
		/// <param name="obj"></param>
		/// <returns></returns>
		/// <remarks>Send an excludsion list to ignore</remarks>
		public bool NotEmpty(T obj)
		{
			// get all the properties in the class
			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return true;

			// check each string property
			foreach ( PropertyInfo prop in props )
			{
				if ( prop.CanRead && prop.PropertyType == typeof( string ) )
				{
					string value = prop.GetValue( obj ).ToString();

					if ( String.IsNullOrWhiteSpace(value) )
						return false;
				}
			}

			return true;
		}
	}
}
