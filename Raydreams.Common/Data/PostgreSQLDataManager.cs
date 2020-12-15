//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text.RegularExpressions;
//using Npgsql;
//using Npgsql.LegacyPostgis;

//namespace Raydreams.Common.Data
//{
//	/// <summary></summary>
//	public class GeoPoint
//	{
//		public double Latitude { get; set; }
//		public double Longitude { get; set; }
//	}

//	/// <summary></summary>
//	public abstract class PostgreSQLDataManager
//	{
//		#region [Fields]

//		private static readonly string _dbInfo = @"SELECT version() AS ProductVersion, inet_server_addr() AS IP";

//		private static readonly string _truncateTable = "TRUNCATE TABLE {0}";
//		private static readonly string _backupTable = "INSERT INTO {0} SELECT * FROM {1}";

//		private NpgsqlConnection _dbConn = null;

//		#endregion [Fields]

//		#region [Constructors]

//		/// <summary>Defers setting the connection string to later in derived classes</summary>
//		protected PostgreSQLDataManager()
//		{
//		}

//		/// <summary></summary>
//		public PostgreSQLDataManager( string connStr )
//		{
//			this._dbConn = new NpgsqlConnection( connStr );
//			NpgsqlConnection.GlobalTypeMapper.UseLegacyPostgis();

//		}

//		#endregion [Constructors]

//		#region [Properties]

//		/// <summary></summary>
//		public NpgsqlConnection DBConnection
//		{
//			get { return this._dbConn; }
//			protected set { this._dbConn = value; }
//		}

//		#endregion [Properties]

//		/// <summary></summary>
//		/// <returns></returns>
//		public string DBInfo()
//		{
//			NpgsqlCommand cmd = new NpgsqlCommand( _dbInfo, this.DBConnection );
//			List<Dictionary<string, string>> dt = this.Select( cmd );

//			if ( dt.Count < 1 )
//				return null;

//			string server = dt[0]["ip"];
//			string ver = dt[0]["productversion"];

//			return String.Format( "DB Server : {0}; SQL Version : {1}", server, ver );
//		}

//		/// <summary>Selects everything from the specified table.</summary>
//		protected List<T> SelectAll<T>( string tableName, string context = null ) where T : new()
//		{
//			// validate the table name, really need a Regex
//			if ( String.IsNullOrWhiteSpace( tableName ) || tableName.Contains( ';' ) )
//				return null;

//			string query = String.Format( "SELECT * FROM {0}", tableName );
//			NpgsqlCommand command = new NpgsqlCommand( query, this.DBConnection );

//			return this.Select<T>( command, context );
//		}

//		/// <summary>Selects using the specified query</summary>
//		/// <returns>Returns each record as a String, String Dictionary. Consumer will have to parse.</returns>
//		protected List<Dictionary<string, string>> Select( NpgsqlCommand cmd )
//		{
//			List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();

//			NpgsqlDataReader reader = null;

//			try
//			{
//				this.DBConnection.Open();

//				reader = cmd.ExecuteReader();

//				// did we get any columns at all
//				if ( reader.FieldCount < 1 )
//					return results;

//				// get the headers
//				List<string> headers = Enumerable.Range( 0, reader.FieldCount ).Select( reader.GetName ).ToList();

//				while ( reader.Read() )
//				{
//					Dictionary<string, string> row = new Dictionary<string, string>();

//					// iterate all the fields
//					for ( int col = 0; col < headers.Count; ++col )
//					{
//						string value = String.Empty;

//						if ( reader[col] != Convert.DBNull )
//							value = reader[col].ToString().Trim();

//						row.Add( headers[col], value );
//					}

//					results.Add( row );
//				}
//			}
//			catch ( System.Exception exp )
//			{
//				throw exp;
//			}
//			finally
//			{
//				// close the reader and connection
//				reader.Close();
//				this.DBConnection.Close();
//			}

//			return results;
//		}

//		/// <summary>Selects using the specified query and maps to a list of object T</summary>
//		/// <param name="context">The context to use but can be set to null or empty string to use a default FieldSource. Don't use any FieldSource attributes to map directly from a property name.</param>
//		protected List<T> Select<T>( NpgsqlCommand cmd, string context = null ) where T : new()
//		{
//			List<T> results = new List<T>();

//			NpgsqlDataReader reader = null;

//			// does any property posses FieldSource with null context
//			bool withContext = (!String.IsNullOrWhiteSpace( context ));

//			// get all the properties in the class
//			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );
//			IEnumerable<PropertyInfo> attrs = props.Where( prop => prop.GetCustomAttributes<FieldSourceAttribute>( true ).Where( attr => String.IsNullOrWhiteSpace( attr.Context ) ).Count() > 0 );

//			if ( !withContext && attrs.Count() > 0 )
//				withContext = true;

//			try
//			{
//				this.DBConnection.Open();

//				reader = cmd.ExecuteReader();

//				T emp = default( T );

//				while ( reader.Read() )
//				{
//					// read a new record from the DB
//					if ( withContext )
//						emp = this.ParseRecord<T>( reader, context );
//					else
//						emp = this.ParseRecord<T>( reader );

//					results.Add( emp );

//					if ( emp == null )
//						continue;
//				}
//			}
//			catch ( System.Exception exp )
//			{
//				throw exp;
//			}
//			finally
//			{
//				// close the reader and connection
//				reader.Close();
//				this.DBConnection.Close();
//			}

//			return results;
//		}

//		/// <summary>Parses to an exact matching property name.</summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="cursor"></param>
//		/// <returns></returns>
//		private T ParseRecord<T>( NpgsqlDataReader cursor ) where T : new()
//		{
//			// valid DB record
//			if ( cursor == null || !cursor.HasRows )
//				return default( T );

//			// get ALL the DB field names lowered
//			List<string> fieldNames = Enumerable.Range( 0, cursor.FieldCount ).Select( cursor.GetName ).ToList();

//			// get all the properties in the class
//			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

//			if ( props.Length < 1 )
//				return default( T );

//			T obj = new T();

//			// set the value of each property on the object
//			foreach ( PropertyInfo prop in props )
//			{
//				string source = prop.Name;

//				// can you publicly set the property
//				if ( !prop.CanWrite )
//					continue;

//				// verify there is a coresponding DB column name
//				if ( !fieldNames.Contains( source ) )
//					continue;

//				this.Parse<T>( obj, prop, cursor, source );
//			}

//			return obj;
//		}

//		/// <summary>Parses to FieldSource adorned attributes.</summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="cursor"></param>
//		/// <param name="context">The context to use to parse, can be null and will use the first FieldSource attribute with no context.</param>
//		/// <returns></returns>
//		private T ParseRecord<T>( NpgsqlDataReader cursor, string context = null ) where T : new()
//		{
//			// valid DB record
//			if ( cursor == null || !cursor.HasRows )
//				return default( T );

//			// get ALL the DB field names lowered
//			List<string> fieldNames = Enumerable.Range( 0, cursor.FieldCount ).Select( cursor.GetName ).ToList(); //.ConvertAll( d => d.ToLower() ); 

//			// get all the properties in the class
//			PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

//			if ( props.Length < 1 )
//				return default( T );

//			T obj = new T();

//			// iterate each property
//			foreach ( PropertyInfo prop in props )
//			{
//				// can you publicly set the property
//				if ( !prop.CanWrite )
//					continue;

//				// get the properties FieldMap attribute
//				List<FieldSourceAttribute> sources = prop.GetCustomAttributes<FieldSourceAttribute>( false ).ToList();

//				// if there is not field map source, then this property is not read from the CSV
//				if ( sources == null || sources.Count < 1 )
//					continue;

//				FieldSourceAttribute map = null;

//				// if the context is null - then use the first empty FieldSource
//				if ( String.IsNullOrWhiteSpace( context ) )
//					map = sources.Where( s => String.IsNullOrWhiteSpace( s.Context ) ).FirstOrDefault();
//				else
//					map = sources.Where( s => s.Context.Equals( context, StringComparison.Ordinal ) ).FirstOrDefault();

//				if ( map == null || String.IsNullOrWhiteSpace( map.Source ) )
//					continue;

//				// verify there is a coresponding DB column name
//				if ( !fieldNames.Contains( map.Source ) )
//					continue;

//				this.Parse<T>( obj, prop, cursor, map.Source );
//			}

//			return obj;
//		}

//		/// <summary>Does the actual conversion</summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="obj">The object we are populating</param>
//		/// <param name="prop">The property we area reading.</param>
//		/// <param name="cursor">The data reader</param>
//		/// <param name="source">The data source field name</param>
//		private void Parse<T>( T obj, PropertyInfo prop, NpgsqlDataReader cursor, string source )
//		{
//			// use the source field to get the value from the dictionary
//			if ( prop.PropertyType == typeof( string ) )
//			{
//				prop.SetValue( obj, cursor.GetStringValue( source ) );
//			}
//			else if ( prop.PropertyType == typeof( DateTime ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, DateTime.MinValue );
//			}
//			else if ( prop.PropertyType == typeof( Nullable<DateTime> ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, null );
//			}
//			else if ( prop.PropertyType == typeof( int ) || prop.PropertyType == typeof( long ) || prop.PropertyType == typeof( double ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, 0 );
//			}
//			else if ( prop.PropertyType == typeof( Nullable<int> ) || prop.PropertyType == typeof( Nullable<long> ) || prop.PropertyType == typeof( Nullable<double> ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, null );
//			}
//			else if ( prop.PropertyType == typeof( bool ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, false );
//			}
//			else if ( prop.PropertyType == typeof( Nullable<bool> ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, null );
//			}
//			else if ( prop.PropertyType == typeof( Guid ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, Guid.Empty );
//			}
//			else if ( prop.PropertyType == typeof( Nullable<Guid> ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, null );
//			}
//			else if ( prop.PropertyType == typeof( DateTimeOffset ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, DateTimeOffset.MinValue );
//			}
//			else if ( prop.PropertyType == typeof( Nullable<DateTimeOffset> ) )
//			{
//				if ( cursor.HasColumn( source, false ) && cursor[source] != Convert.DBNull )
//					prop.SetValue( obj, cursor[source] );
//				else
//					prop.SetValue( obj, null );
//			}
//			else if ( prop.PropertyType == typeof( GeoPoint ) )
//			{
//				if ( cursor.HasColumn( source, false ) )
//				{
//					var point = cursor.GetFieldValue<PostgisPoint>( cursor.GetOrdinal( source ) );
//					prop.SetValue( obj, new GeoPoint() { Latitude = point.Y, Longitude = point.X } );
//				}
//			}
//			else if ( prop.PropertyType.IsEnum )
//			{
//				Type t = prop.PropertyType;

//				// convert to a string value
//				string val = cursor.GetStringValue( source );

//				// upcast the string
//				Object temp = Enum.Parse( t, val, true );

//				//prop.SetValue( obj, temp );

//				// if the parsed value is not null then act normal
//				if ( temp != null )
//				{
//					prop.SetValue( obj, temp );
//				}
//				else // if the enum type is nullable, then set to null else set to default(T)
//				{
//					if ( t == typeof( Nullable<> ) )
//						prop.SetValue( obj, null );
//					else
//						prop.SetValue( obj, Activator.CreateInstance( t ) );
//				}
//			}
//			else
//			{
//				throw new System.Exception( "Type not yet supported." );
//			}
//		}

//		/// <summary></summary>
//		/// <param name="t"></param>
//		/// <returns></returns>
//		private static bool QuotableType( Type t )
//		{
//			if ( t == typeof( string ) )
//				return true;
//			else if ( t == typeof( DateTime ) || t == typeof( Nullable<DateTime> ) )
//				return true;
//			else if ( t == typeof( DateTimeOffset ) || t == typeof( Nullable<DateTimeOffset> ) )
//				return true;
//			else if ( t == typeof( Guid ) || t == typeof( Nullable<Guid> ) )
//				return true;

//			return false;
//		}

//		/// <summary>Searches a query and replaces all the table name tokens with the corresponding property string.</summary>
//		/// <returns></returns>
//		/// <remarks>
//		/// SELECT * FROM {{MyTable}}
//		/// {{MyTable}} will be replaced by the class property with the name MyTableName. Observe that 'Name' is left off the token.
//		/// try to make it work with MyViewName as well
//		/// </remarks>
//		protected string ReplaceTableNames( string query )
//		{
//			if ( String.IsNullOrWhiteSpace( query ) )
//				return String.Empty;

//			PropertyInfo[] props = this.GetType().GetProperties();

//			Regex pattern = new Regex( @"[{]{2}([a-zA-Z]+)[}]{2}", RegexOptions.IgnoreCase );

//			MatchCollection mc = pattern.Matches( query );

//			foreach ( Match m in mc )
//			{
//				// get the table name
//				string tableName = m.Groups[1].Value;

//				// find a property with the name
//				PropertyInfo prop = props.Where( p => p.Name == String.Format( "{0}Name", tableName ) ).FirstOrDefault();

//				if ( prop == null || !prop.CanRead || prop.PropertyType != typeof( string ) )
//					continue;

//				// get the value name
//				string replace = prop.GetValue( this ).ToString();

//				if ( String.IsNullOrWhiteSpace( replace ) )
//					continue;

//				// replace the token
//				query = query.Replace( String.Format( "{{{{{0}}}}}", tableName ), replace );
//			}

//			return query;
//		}

//		/// <summary>Trncates the destination table.</summary>
//		/// <param name="srcTable">Tabel to copy from</param>
//		/// <param name="bkupTable">Table to truncate and copy to</param>
//		protected virtual int BackupTable( string srcTable, string bkupTable )
//		{
//			if ( String.IsNullOrWhiteSpace( srcTable ) || String.IsNullOrWhiteSpace( bkupTable ) )
//				return 0;

//			// should check the fields all match

//			// first truncate the dest
//			string q1 = String.Format( _truncateTable, bkupTable );
//			NpgsqlCommand cmd1 = new NpgsqlCommand( q1, this.DBConnection );

//			this.Execute( cmd1 );

//			// now copy
//			string q2 = String.Format( _backupTable, bkupTable, srcTable );


//			NpgsqlCommand cmd2 = new NpgsqlCommand( q2, this.DBConnection );

//			return this.Execute( cmd2 );
//		}

//		/// <summary></summary>
//		protected int Execute( NpgsqlCommand cmd )
//		{
//			int rows = 0;

//			try
//			{
//				this.DBConnection.Open();

//				rows = cmd.ExecuteNonQuery();

//			}
//			catch ( System.Exception exp )
//			{
//				throw exp;
//			}
//			finally
//			{
//				// close the reader and connection
//				this.DBConnection.Close();
//			}

//			return rows;
//		}

//	}
//}
