using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Raydreams.Common.Extensions
{
	/// <summary></summary>
	public static class ListExtensions
	{
		/// <summary>Given a list of objects and a file path, will write out all the properties values of each object in the list. Mainly used for debugging and reporting.</summary>
		/// <remarks>Mainly to dump a collection of objects to a file.</remarks>
		public static int WriteObjectList<T>(this List<T> objects, string filePath, char delim = ';', bool quoteFields = false)
		{
			StringBuilder sb = new StringBuilder();
			int rec = 0;

			// get all the properties in the class
			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

			if ( props.Length < 1 )
				return rec;

			// open a file
			using ( StreamWriter sw = new StreamWriter( filePath, false ) )
			{

				// get headers
				foreach ( PropertyInfo property in props )
				{
					if ( property.CanRead )
					{
						sb.AppendFormat( "{2}{0}{2}{1}", property.Name, delim, (quoteFields) ? "\"": String.Empty );
					}
				}

				// write the headers
				sb.Length = sb.Length - 1;
				sw.WriteLine( sb.ToString() );
				sb.Length = 0;

				foreach ( T item in objects )
				{
					foreach ( PropertyInfo property in props )
					{
						if ( property.CanRead )
						{
							object value = property.GetValue( item );

							sb.AppendFormat( "{2}{0}{2}{1}", ( value == null ) ? String.Empty : value.ToString(), delim, ( quoteFields ) ? "\"" : String.Empty );
						}
					}

					// write values
					sb.Length = sb.Length - 1;
					sw.WriteLine( sb.ToString() );
					sb.Length = 0;
					++rec;
				}

				sw.Flush();
			}

			return rec;
		}
	}
}
