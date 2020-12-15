using Raydreams.Common.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.Extensions
{
	/// <summary></summary>
	public static class ObjectExtensions
	{
		/// <summary>Converts the entire object to a key/value pair string</summary>
		public static string ToKeyValuePair<T>( T obj, string delim = ";" )
		{
			if ( obj == null )
				return String.Empty;

			if ( String.IsNullOrWhiteSpace( delim ) )
				delim = String.Empty;

			// get all the properties in the class
			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return String.Empty;

			StringBuilder sb = new StringBuilder();

			foreach ( PropertyInfo property in props )
			{
				if ( property.CanRead )
				{
					object value = property.GetValue( obj );

					sb.AppendFormat( "{0}={1}{2}", property.Name, ( value == null ) ? String.Empty : value.ToString(), delim );
				}
			}

			return sb.ToString();
		}

		/// <summary>Given an istance of an object T, create a hash for field values we want to track a change in</summary>
		/// <typeparam name="T">Object type</typeparam>
		/// <param name="obj">object instance</param>
		/// <returns></returns>
		/// <remarks>
		/// https://developer.apple.com/library/content/documentation/Swift/Conceptual/Swift_Programming_Language/AdvancedOperators.html
		/// https://msdn.microsoft.com/en-us/library/system.object.gethashcode(v=vs.110).aspx
		/// </remarks>
		public static int HashByValue<T>(T obj)
		{
			// get only public instance properties
			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

			// track a running hash value
			int hash = 0;

			foreach ( PropertyInfo prop in props )
			{
				// ignore if not a public read property
				if ( !prop.CanRead )
					continue;

				// get the properties TrackChange attribute
				TrackAttribute map = prop.GetCustomAttribute<TrackAttribute>( false );

				// has a Track attribute set to true
				if ( map != null && map.TrackChange )
				{
					object value = prop.GetValue( obj, null );

					if ( value == null )
						hash = hash ^ 0;
					// make null strings and empty strings cause the same value since they might be read from different sources
					else if ( prop.PropertyType == typeof( string ) && String.IsNullOrWhiteSpace( value.ToString() ) )
						hash = hash ^ 0;
					else
						hash = hash ^ value.GetHashCode();
				}
			}

			return hash;
		}

		/// <summary>Forms a string to determine if the employee is the same</summary>
		public static string HashByString<T>( T obj )
		{
			// get all the properties in the class
			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

			// concatenate every field we care about
			StringBuilder sb = new StringBuilder();

			foreach ( PropertyInfo prop in props )
			{
				// is this a public read property
				if ( !prop.CanRead )
					continue;

				// get the properties FieldMap attribute
				TrackAttribute map = prop.GetCustomAttribute<TrackAttribute>( false );

				if ( map != null && map.TrackChange )
				{
					object value = prop.GetValue( obj, null );

					if ( value == null )
						continue;

					if ( prop.PropertyType == typeof( string ) )
					{
						sb.Append( value.ToString().Trim().ToLower() );
					}
					else if ( prop.PropertyType == typeof( bool ) || prop.PropertyType == typeof( Nullable<bool> ) )
					{
						sb.Append( value.ToString().ToLower()[0] );
					}
					else if ( prop.PropertyType == typeof( DateTime ) || prop.PropertyType == typeof( Nullable<DateTime> ) )
					{
						sb.Append( value.ToString().ToLower() );
					}
					else if ( prop.PropertyType == typeof( int ) || prop.PropertyType == typeof( Nullable<int> ) )
					{
						sb.Append( value.ToString() );
					}
					else if ( prop.PropertyType == typeof( long ) || prop.PropertyType == typeof( Nullable<long> ) )
					{
						sb.Append( value.ToString() );
					}
					else if ( prop.PropertyType.IsEnum )
					{
						sb.Append( Convert.ToInt32( value ) );
					}
					else
					{
						throw new System.Exception( "Unknown type in the Hash Comparison." );
					}

				}
			}

			return sb.ToString();
		}

	}
}


