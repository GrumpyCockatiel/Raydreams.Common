using Raydreams.Common.Data;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace Raydreams.Common.Extensions
{
	/// <summary>Dictionary extensions</summary>
	public static class DictionaryExtensions
	{
		/// <summary>Expands a distribution dictionary in the form value,occurances to a single int array</summary>
        /// <param name="dist"></param>
        /// <returns></returns>
		public static int[] FunctionExpando( this Dictionary<int, int> dist )
		{
			int len = dist.Sum( i => i.Value );
			int[] func = new int[len];

			int i = 0;
			foreach ( KeyValuePair<int, int> kvp in dist )
			{
				int[] temp = Enumerable.Repeat( kvp.Key, kvp.Value ).ToArray();
				Array.Copy( temp, 0, func, i, temp.Length );
				i += temp.Length;
			}

			return func;
		}

		/// <summary>Converts a dictionary to a DataTable</summary>
		/// <typeparam name="TKey"></typeparam>
		/// <typeparam name="TValue"></typeparam>
		/// <param name="currentDictionary"></param>
		/// <returns></returns>
		public static DataSet ToDataSet<TKey, TValue>( this IDictionary<TKey, TValue> currentDictionary )
		{
			DataSet exportedDataSet = new DataSet();

			DataTable exportedDataTable = exportedDataSet.Tables.Add();

			foreach ( TKey key in currentDictionary.Keys )
			{
				exportedDataTable.Columns.Add( key.ToString() );
			}

			DataRow newRow = exportedDataTable.NewRow();

			foreach ( KeyValuePair<TKey, TValue> entry in currentDictionary )
			{
				string key = entry.Key.ToString();

				newRow[key] = ( entry.Value != null ) ? entry.Value.ToString() : String.Empty;
			}

			exportedDataSet.Tables[0].Rows.Add( newRow );

			return exportedDataSet;
		}

		/// <summary>Takes a string, string dictionary of field/value pairs and converts them to the T source object</summary>
		/// <param name="values"></param>
		/// <param name="context">The context to use, if null then use every readable property</param>
		/// <returns>Returns new object of type T</returns>
        /// <remarks>Uses RayProperty to find the annotations
        /// Only used in DataFileReader for now
        /// </remarks>
		public static T SourceToObject<T>( Dictionary<string, string> values, string context ) where T : class, new()
		{
			// get all the properties in the class
			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return null;

			// cretae a new instance of T
			T obj = new T();

			// iterate each property in the type and see if it has a matching field in the source file
			foreach ( PropertyInfo prop in props )
			{
				// can you publicly set the property
				if ( !prop.CanWrite )
					continue;

				string value = null;

				// if the context is null look for a property with this header name
				if ( String.IsNullOrWhiteSpace( context ) )
				{
					// if the file contains the field
					if ( values.Keys.Contains( prop.Name ) )
						value = ( String.IsNullOrWhiteSpace( values[prop.Name] ) ) ? String.Empty : values[prop.Name].Trim();
				}
				else
				{
					// get the properties FieldMap attribute
					List<RayPropertyAttribute> sources = prop.GetCustomAttributes<RayPropertyAttribute>( false ).ToList();

					// if there is no field map source, then this property is not read from the CSV
					if ( sources == null || sources.Count < 1 )
						continue;

					RayPropertyAttribute map = sources.Where( s => s.Context.Equals( context, StringComparison.Ordinal ) ).FirstOrDefault();

					// the context does not match any attribute on the class
					if ( map == null || String.IsNullOrWhiteSpace( map.Source ) )
						continue;

					// does the source file have this field
					if ( values.Keys.Contains( map.Source ) )
						value = ( String.IsNullOrWhiteSpace( values[map.Source] ) ) ? String.Empty : values[map.Source].Trim();
				}

				// use the source field to get the value from the dictionary
				if ( prop.PropertyType == typeof( string ) )
				{
					prop.SetValue( obj, value );
				}
				else if ( prop.PropertyType == typeof( DateTime ) )
				{
					prop.SetValue( obj, value.GetDateTimeValue() );
				}
				else if ( prop.PropertyType == typeof( Nullable<DateTime> ) )
				{
					prop.SetValue( obj, value.GetNullDateTimeValue() );
				}
				else if ( prop.PropertyType == typeof( int ) )
				{
					prop.SetValue( obj, value.GetIntValue() );
				}
				else if ( prop.PropertyType == typeof( double ) )
				{
					prop.SetValue( obj, value.GetDoubleValue() );
				}
				else if ( prop.PropertyType == typeof( Nullable<int> ) )
				{
					prop.SetValue( obj, value.GetNullableIntValue() );
				}
				else if ( prop.PropertyType == typeof( long ) )
				{
					prop.SetValue( obj, value.GetLongValue() );
				}
				else if ( prop.PropertyType == typeof( bool ) )
				{
					prop.SetValue( obj, value.GetBooleanValue() );
				}
				else if ( prop.PropertyType.IsEnum )
				{
					Type t = prop.PropertyType;

					//prop.SetValue( obj, cursor.GetStringValue( key ).GetEnumValue<T.GetType()>() );

					Object temp = Enum.Parse( t, value, true );
					prop.SetValue( obj, temp );
				}
				else if ( prop.PropertyType == typeof( ObjectId ) )
				{
					if ( String.IsNullOrWhiteSpace( value ) )
					{
						prop.SetValue( obj, ObjectId.Empty );
					}
					else
						prop.SetValue( obj, new ObjectId(value) );
				}
				else
				{
					throw new System.Exception( "Type not yet supported." );
				}
			}

			return obj;
		}
	}
}
