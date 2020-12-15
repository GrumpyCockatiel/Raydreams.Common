using System;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Raydreams.Common.Data
{
	/// <summary></summary>
	public static class DbReaderExtensions
	{
		/// <summary>Does the SqlDataReader have a column with the specified column name</summary>
		/// <param name="dr">The data reader</param>
		/// <param name="colName">The name of the column</param>
		/// <param name="ignoreCase">Match on case or not</param>
		/// <returns></returns>
		public static bool HasColumn( this DbDataReader dr, string colName, bool ignoreCase = false )
		{
			StringComparison options = (ignoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

			return Enumerable.Range( 0, dr.FieldCount ).Any( i => string.Equals( dr.GetName( i ), colName, options ) );
		}

		/// <summary>Converts a SQL DB Field value to the object value. Needs to cover all types</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj">Object instance to write value to</param>
		/// <param name="prop">The property to write to</param>
		/// <param name="cursor">The DB reader</param>
		/// <param name="source">The column source field</param>
		/// <remarks>prop.SetValue( obj, cursor.GetStringValue(source).GetIntValue() );</remarks>
		public static void Parse<T>( this DbDataReader cursor, T obj, PropertyInfo prop, string source )
		{
			// string
			if ( prop.PropertyType == typeof( string ) )
			{
				prop.SetValue( obj, cursor.GetStringValue( source ) );
			}
			// DateTime
			else if ( prop.PropertyType == typeof( DateTime ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, DateTime.MinValue );
			}
			// DateTime Nullable
			else if ( prop.PropertyType == typeof( Nullable<DateTime> ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, null );
			}
			// long int and double
			else if ( prop.PropertyType == typeof( int ) || prop.PropertyType == typeof( long ) || prop.PropertyType == typeof( double ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, 0 );
			}
			// nullable long int and double
			else if ( prop.PropertyType == typeof( Nullable<int> ) || prop.PropertyType == typeof( Nullable<long> ) || prop.PropertyType == typeof( Nullable<double> ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, null );
			}
			// bool 
			else if ( prop.PropertyType == typeof( bool ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, false );
			}
			// nullable bool
			else if ( prop.PropertyType == typeof( Nullable<bool> ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, null );
			}
			// GUID
			else if ( prop.PropertyType == typeof( Guid ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, Guid.Empty );
			}
			// nullable guid
			else if ( prop.PropertyType == typeof( Nullable<Guid> ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, null );
			}
			// datetime offset
			else if ( prop.PropertyType == typeof( DateTimeOffset ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, DateTimeOffset.MinValue );
			}
			// nullable datetime offset
			else if ( prop.PropertyType == typeof( Nullable<DateTimeOffset> ) )
			{
				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
					prop.SetValue( obj, cursor[source] );
				else
					prop.SetValue( obj, null );
			}
			// enum
			else if ( prop.PropertyType.IsEnum )
			{
				Type t = prop.PropertyType;

				// convert to a string value
				string val = cursor.GetStringValue( source );

				// upcast the string
				Object temp = Enum.Parse( t, val, true );

				//prop.SetValue( obj, temp );

				// if the parsed value is not null then act normal
				if ( temp != null )
				{
					prop.SetValue( obj, temp );
				}
				else // if the enum type is nullable, then set to null else set to default(T)
				{
					if ( t == typeof( Nullable<> ) )
						prop.SetValue( obj, null );
					else
						prop.SetValue( obj, Activator.CreateInstance( t ) );
				}
			}
			else
			{
				throw new System.Exception( "Type not yet supported." );
			}
		}

		/// <summary>Extension to DB Data Reader that converts a SQL data field to a string</summary>
		/// <param name="reader">The cursor</param>
		/// <param name="colName">The DB column name</param>
		/// <returns>Returns the value as as string or null if no column found or the string itself was null or empty</returns>
		public static string GetStringValue( this DbDataReader reader, string colName )
		{
			//reader.GetSchemaTable().Columns.Contains
			int col = -1;

			try
			{
				col = reader.GetOrdinal( colName );
			}
			catch ( IndexOutOfRangeException )
			{
				return null;
			}

			if ( col < 0 || reader[col] == Convert.DBNull )
				return null;

			string temp = null;

			// is the DB type a DateTime type in .NET
			if ( reader[col] is DateTime )
			{
				DateTime t = Convert.ToDateTime( reader[col] );
				temp = t.ToString();
			}
			else
				temp = reader[col].ToString().Trim();

			if ( String.IsNullOrWhiteSpace( temp ) )
				return null;

			return temp;
		}

		/// <summary>Returns the Type of a DB field in .NET types</summary>
		/// <param name="reader"></param>
		/// <param name="colName"></param>
		/// <returns></returns>
		/// <remarks>
		/// if (reader[col] is DateTime) ; // straight compare to a type
		/// reader.GetDataTypeName(col); // get the type as a string like varchar
		/// </remarks>
		public static Type GetColumnType( this DbDataReader reader, string colName )
		{
			//reader.GetSchemaTable().Columns.Contains
			int col = -1;

			try
			{
				col = reader.GetOrdinal( colName );
			}
			catch ( IndexOutOfRangeException )
			{
				return null;
			}

			return reader.GetFieldType( col );

		}

	}
}
