using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Data
{
    /// <summary>A base data manager for SQLlite</summary>
    /// <remarks>Still working on bring the SQL Server, postGres and SQLite up to all the same features</remarks>
    public abstract class SQLiteDataManager
    {
        #region [Fields]

        /// <summary>Gets info about the DB</summary>
        private static readonly string _dbInfo = @"SELECT SERVERPROPERTY('productversion') AS [ProductVersion], SERVERPROPERTY('productlevel') AS [ProductLevel], SERVERPROPERTY('edition') AS [Edition], SERVERPROPERTY('EditionID') AS [EditionID], SERVERPROPERTY('ServerName') AS [MachineName]";

        /// <summary>Base command for truncating a table</summary>
        private static readonly string _truncateTable = "TRUNCATE TABLE {0}";

        /// <summary>Base command for bulk inserting from one table to another</summary>
        private static readonly string _backupTable = "INSERT INTO {0} SELECT * FROM {1}";

        #endregion [Fields]

        #region [Constructors]

        /// <summary></summary>
        public SQLiteDataManager(string connStr)
        {
            if ( String.IsNullOrWhiteSpace( connStr ) )
                throw new System.ArgumentException( "A connection string is required", nameof(connStr) );

            this.DBConnection = new SQLiteConnection(connStr);
        }

        #endregion [Constructors]

        #region [Properties]

        /// <summary>The DB Connection</summary>
        public SQLiteConnection DBConnection { get; set; }

        #endregion [Properties]

        #region [ Methods ]

        /// <summary>Selects everything from the specified table.</summary>
        protected List<T> SelectAll<T>(string tableName, string context = null) where T : new()
        {
            // validate the table name, really need a Regex
            if (String.IsNullOrWhiteSpace(tableName) || tableName.Contains(';'))
                return null;

            string query = String.Format("SELECT * FROM {0}", tableName);
            SQLiteCommand command = new SQLiteCommand(query, this.DBConnection);

            return this.Select<T>(command, context);
        }

        /// <summary>Selects using the specified query</summary>
        /// <returns>Returns each record as a String, String Dictionary. Consumer will have to parse.</returns>
        protected List<Dictionary<string, string>> Select(SQLiteCommand cmd)
        {
            List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();

            SQLiteDataReader reader = null;

            try
            {
                this.DBConnection.Open();

                reader = cmd.ExecuteReader();

                // did we get any columns at all
                if (reader.FieldCount < 1)
                    return results;

                // get the headers
                List<string> headers = Enumerable.Range(0, reader.FieldCount).Select(reader.GetName).ToList();

                while (reader.Read())
                {
                    Dictionary<string, string> row = new Dictionary<string, string>();

                    // iterate all the fields
                    for (int col = 0; col < headers.Count; ++col)
                    {
                        string value = String.Empty;

                        if (reader[col] != Convert.DBNull)
                            value = reader[col].ToString().Trim();

                        row.Add(headers[col], value);
                    }

                    results.Add(row);
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                // close the reader and connection
                reader.Close();
                this.DBConnection.Close();
            }

            return results;
        }

        /// <summary>Selects using the specified query and maps to a list of object T</summary>
        /// <param name="context">The context to use but can be set to null or empty string to use a default FieldSource. Don't use any FieldSource attributes to map directly from a property name.</param>
        protected List<T> Select<T>(SQLiteCommand cmd, string context = null) where T : new()
        {
            List<T> results = new List<T>();

            SQLiteDataReader reader = null;

            // does any property posses RayAttribute with null context
            bool withContext = (!String.IsNullOrWhiteSpace(context));

            // get all the properties in the class
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            IEnumerable<PropertyInfo> attrs = props.Where(prop => prop.GetCustomAttributes<RayPropertyAttribute>(true).Where(attr => String.IsNullOrWhiteSpace(attr.Context)).Count() > 0);

            if (!withContext && attrs.Count() > 0)
                withContext = true;

            try
            {
                this.DBConnection.Open();

                reader = cmd.ExecuteReader();

                T emp = default(T);

                while (reader.Read())
                {
                    // read a new record from the DB
                    if (withContext)
                        emp = this.ParseRecord<T>(reader, context);
                    else
                        emp = this.ParseRecord<T>(reader);

                    results.Add(emp);

                    if (emp == null)
                        continue;
                }
            }
            catch (System.Exception exp)
            {
                throw exp;
            }
            finally
            {
                // close the reader and connection
                reader.Close();
                this.DBConnection.Close();
            }

            return results;
        }

        /// <summary>Insert the generic object into the specified database table where Property Names exactly match the field names.</summary>
        /// <param name="obj">The object to insert</param>
        /// <param name="tableName">The table to insert into</param>
        /// <param name="context">WHat context to use which may be null or named</param>
        /// <returns>Number of inserted rows</returns>
        /// <remarks>Right now casts enums to an Int until we have some way to indicate how to handle them</remarks>
        protected int Insert<T>( T obj, string tableName ) where T : new()
        {
            // if no table or connection then abort
            if ( String.IsNullOrWhiteSpace( tableName ) || this.DBConnection == null )
                return 0;

            // get all the properties in the class
            PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

            // if no readable properties then abort
            if ( props.Length < 1 )
                return 0;

            // roll the query & execute the command
            SQLiteCommand cmd = this.FormatInsertCommand<T>( obj, tableName, props, null );
            cmd.Connection = this.DBConnection;
            return this.Execute( cmd );
        }

        /// <summary>Insert an object with context</summary>
        /// <param name="obj"></param>
        /// <param name="tableName"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        /// <remarks>Inserts can be merged into 1 method at some point</remarks>
        protected int Insert<T>( T obj, string tableName, string context ) where T : new()
        {
            // if no table or connection then abort
            if ( String.IsNullOrWhiteSpace( tableName ) || this.DBConnection == null )
                return 0;

            // determine the context
            bool isNullContext = String.IsNullOrWhiteSpace( context );
            if ( !isNullContext ) context = context.Trim();

            // get all the properties in the class
            PropertyInfo[] props = typeof( T ).GetProperties( BindingFlags.Public | BindingFlags.Instance );

            // if no readable properties then abort - possible branch to insert by exact name
            if ( props.Length < 1 )
                return 0;

            // get properties either with null context or context by name
            IEnumerable<PropertyInfo> attrs = props.Where( prop => prop.GetCustomAttributes<RayPropertyAttribute>( true )
                .Where( attr => (isNullContext) ? String.IsNullOrWhiteSpace( attr.Context ) : attr.Context == context ).Count() > 0 );

            // possible branch to insert by exact name
            if ( attrs.Count() < 1 )
                return 0;

            // roll the query & execute
            SQLiteCommand cmd = this.FormatInsertCommand<T>( obj, tableName, attrs, context );
            cmd.Connection = this.DBConnection;
            return this.Execute( cmd );
        }

        /// <summary>Executes a non query command</summary>
        protected int Execute( SQLiteCommand cmd )
        {
            int rows = 0;

            try
            {
                cmd.Connection.Open();
                rows = cmd.ExecuteNonQuery();
            }
            catch ( System.Exception exp )
            {
                throw exp;
            }
            finally
            {
                // close the reader and connection
                cmd.Connection.Close();
            }

            return rows;
        }

        /// <summary>Counts the number of records in the table</summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        protected int Count(string tableName)
        {
            int count = 0;

            // if no table or connection then abort
            if ( String.IsNullOrWhiteSpace( tableName ) || this.DBConnection == null )
                return count;

            SQLiteCommand cmd = new SQLiteCommand( $"SELECT COUNT(*) FROM {tableName.Trim()}", this.DBConnection );

            try
            {
                cmd.Connection.Open();
                object results = cmd.ExecuteScalar();

                if ( results != null )
                    count = Convert.ToInt32( results );
            }
            catch ( System.Exception exp )
            {
                throw exp;
            }
            finally
            {
                // close the reader and connection
                cmd.Connection.Close();
            }

            return count;
        }

        /// <summary>Truncates the entire contents of a table, be careful using this.</summary>
        protected virtual int TruncateTable( string tableName )
        {
            if ( String.IsNullOrWhiteSpace( tableName ) )
                return 0;

            SQLiteCommand cmd = new SQLiteCommand( $"DELETE FROM {tableName.Trim()}", this.DBConnection );

            try
            {
                return this.Execute( cmd );
            }
            catch
            {
                return 0;
            }
        }

        #endregion [ Methods ]

        #region [ Private Methods ]

        /// <summary>Formats the insert statement</summary>
        /// <param name="obj"></param>
        /// <param name="props"></param>
        /// <returns>Update to used parameterized queries</returns>
        private SQLiteCommand FormatInsertCommand<T>( T obj, string tableName, IEnumerable<PropertyInfo> props, string ctx )
        {
            // if no table or properties then abort
            if ( obj == null || String.IsNullOrWhiteSpace( tableName ) || props == null || props.Count() < 1 )
                return null;

            // determine the context type from the object itself
            RayContext ctxType = RayPropertyAttribute.CalculateContext( typeof( T ), ctx );

            if ( ctxType == RayContext.Error )
                throw new System.Exception("A context was specified by no properties have RayAttribute");

            Dictionary<string, SQLiteParameter> paras = new Dictionary<string, SQLiteParameter>();

            // set the value of each property on the object
            foreach ( PropertyInfo prop in props )
            {
                // dict values
                string propName = null;
                SQLiteParameter param = null;

                // can you publicly read the property
                if ( !prop.CanRead )
                    continue;

                // reject arrays except byte[]
                if ( prop.PropertyType.IsArray && prop.PropertyType != typeof( byte[] ) )
                    continue;

                // reject most generic Lists for now
                if ( prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof( List<> ) )
                        continue;

                // property names is determined context type
                if ( ctxType == RayContext.PropertyName)
                    propName = prop.Name;
                else if ( ctxType == RayContext.Null )
                {
                    // get the properties FieldMap attribute
                    RayPropertyAttribute dest = prop.GetCustomAttributes<RayPropertyAttribute>( false ).Where( a => a.IsNullContext ).FirstOrDefault();

                    if ( dest == null )
                        continue;

                    propName = dest.Destination.Trim();
                }

                // get the value
                object value = prop.GetValue( obj );

                // any null values just set to DB Null
                if ( value == null )
                {
                    param = new SQLiteParameter( $":{propName}", Convert.DBNull );
                }
                // strings
                else if ( prop.PropertyType == typeof(string) )
                {
                    param = new SQLiteParameter( $":{propName}", (value as string) );
                }
                // Guids
                else if ( prop.PropertyType == typeof( Guid ) || prop.PropertyType == typeof( Nullable<Guid> ) )
                {
                    param = new SQLiteParameter( $":{propName}", ( (Guid)value ).ToString() );
                }
                // dates of any kind
                else if ( prop.PropertyType == typeof( DateTime ) || prop.PropertyType == typeof( Nullable<DateTime> ) )
                {
                    param = new SQLiteParameter( $":{propName}", value );
                }
                else if ( prop.PropertyType == typeof( DateTimeOffset ) || prop.PropertyType == typeof( Nullable<DateTimeOffset> ) )
                {
                    param = new SQLiteParameter( $":{propName}", value );
                }
                // bools are converted to int
                else if ( prop.PropertyType == typeof( bool ) || prop.PropertyType == typeof( Nullable<bool> ) )
                {
                    param = new SQLiteParameter( $":{propName}", (bool)value ? 1 : 0 );
                }
                // enums are int until we create an attribute option to specify text
                else if ( prop.PropertyType.IsEnum )
                {
                    param = new SQLiteParameter( $":{propName}", (int)value );
                }
                // binary blobs
                else if ( prop.PropertyType == typeof( byte[]) )
                {
                    param = new SQLiteParameter( $":{propName}", value );
                }
                else if ( !prop.PropertyType.IsClass ) // everything else thats NOT a class we have not already handled
                    param = new SQLiteParameter( $":{propName}", value );

                // add the whole thing to the dictionary
                if (param != null)
                    paras.Add( propName, param );
            }

            // roll the query
            var parameters = paras.Select( n => n.Value.ParameterName ).ToArray();

            string query = $"INSERT INTO {tableName} ({String.Join( ",", paras.Keys.ToArray() )}) VALUES ({String.Join( ",", parameters)})";

            SQLiteCommand cmd = new SQLiteCommand( query );
            cmd.CommandType = System.Data.CommandType.Text;
            cmd.Parameters.AddRange( paras.Values.ToArray() );

            return cmd;
        }

        /// <summary>Parses to an exact matching property name.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cursor"></param>
        /// <returns></returns>
        private T ParseRecord<T>(SQLiteDataReader cursor) where T : new()
        {
            // valid DB record
            if (cursor == null || !cursor.HasRows)
                return default(T);

            // get ALL the DB field names lowered
            List<string> fieldNames = Enumerable.Range(0, cursor.FieldCount).Select(cursor.GetName).ToList();

            // get all the properties in the class
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (props.Length < 1)
                return default(T);

            T obj = new T();

            // set the value of each property on the object
            foreach (PropertyInfo prop in props)
            {
                string source = prop.Name;

                // can you publicly set the property
                if (!prop.CanWrite)
                    continue;

                // verify there is a coresponding DB column name
                if (!fieldNames.Contains(source))
                    continue;

				cursor.Parse<T>(obj, prop, source);
            }

            return obj;
        }

        /// <summary>Parses to RayAttribute adorned attributes.</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cursor"></param>
        /// <param name="context">The context to use to parse, can be null and will use the first FieldSource attribute with no context.</param>
        /// <returns></returns>
        private T ParseRecord<T>(SQLiteDataReader cursor, string context = null) where T : new()
        {
            // valid DB record
            if (cursor == null || !cursor.HasRows)
                return default(T);

            // get ALL the DB field names lowered
            List<string> fieldNames = Enumerable.Range(0, cursor.FieldCount).Select(cursor.GetName).ToList(); //.ConvertAll( d => d.ToLower() ); 

            // get all the properties in the class
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            if (props.Length < 1)
                return default(T);

            T obj = new T();

            // iterate each property
            foreach (PropertyInfo prop in props)
            {
                // can you publicly set the property
                if (!prop.CanWrite)
                    continue;

                // get the properties FieldMap attribute
                List<RayPropertyAttribute> sources = prop.GetCustomAttributes<RayPropertyAttribute>(false).ToList();

                // if there is not field map source, then this property is not read from the DB
                if (sources == null || sources.Count < 1)
                    continue;

                RayPropertyAttribute map = null;

                // if the context is null - then use the first empty FieldSource
                if (String.IsNullOrWhiteSpace(context))
                    map = sources.Where(s => String.IsNullOrWhiteSpace(s.Context)).FirstOrDefault();
                else
                    map = sources.Where(s => s.Context.Equals(context, StringComparison.Ordinal)).FirstOrDefault();

                if (map == null || String.IsNullOrWhiteSpace(map.Source))
                    continue;

                // verify there is a coresponding DB column name
                if (!fieldNames.Contains(map.Source))
                    continue;

                // calls the generic value parser in DbReaderExtensions
                cursor.Parse<T>(obj, prop, map.Source);
            }

            return obj;
        }

        #endregion [ Private Methods ]
    }
}
