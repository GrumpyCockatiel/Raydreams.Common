using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Data
{
    /// <summary></summary>
    /// <remarks>Can we switch this to RayProperty</remarks>
    public class SQLiteDataManager
    {
        #region [Fields]

        private static readonly string _dbInfo = @"SELECT SERVERPROPERTY('productversion') AS [ProductVersion], SERVERPROPERTY('productlevel') AS [ProductLevel], SERVERPROPERTY('edition') AS [Edition], SERVERPROPERTY('EditionID') AS [EditionID], SERVERPROPERTY('ServerName') AS [MachineName]";

        private static readonly string _truncateTable = "TRUNCATE TABLE {0}";
        private static readonly string _backupTable = "INSERT INTO {0} SELECT * FROM {1}";

        private SQLiteConnection _dbConn = null;

        #endregion [Fields]


        #region [Constructors]

        /// <summary></summary>
        public SQLiteDataManager(string connStr)
        {
            this._dbConn = new SQLiteConnection(connStr);
        }

        #endregion [Constructors]

        #region [Properties]

        /// <summary></summary>
        public SQLiteConnection DBConnection
        {
            get { return this._dbConn; }
            protected set { this._dbConn = value; }
        }

        #endregion [Properties]

        #region [Methods]

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

            // does any property posses FieldSource with null context
            bool withContext = (!String.IsNullOrWhiteSpace(context));

            // get all the properties in the class
            PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            IEnumerable<PropertyInfo> attrs = props.Where(prop => prop.GetCustomAttributes<FieldSourceAttribute>(true).Where(attr => String.IsNullOrWhiteSpace(attr.Context)).Count() > 0);

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

        /// <summary>Parses to FieldSource adorned attributes.</summary>
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
                List<FieldSourceAttribute> sources = prop.GetCustomAttributes<FieldSourceAttribute>(false).ToList();

                // if there is not field map source, then this property is not read from the DB
                if (sources == null || sources.Count < 1)
                    continue;

                FieldSourceAttribute map = null;

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


        /// <summary>Does the actual conversion</summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="prop"></param>
        /// <param name="cursor"></param>
        /// <param name="source"></param>
        //private void Parse<T>(T obj, PropertyInfo prop, SQLiteDataReader cursor, string source)
        //{
        //    // use the source field to get the value from the dictionary
        //    if (prop.PropertyType == typeof(string))
        //    {
        //        prop.SetValue(obj, cursor.GetStringValue(source));
        //    }
        //    else if (prop.PropertyType == typeof(DateTime))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, DateTime.MinValue);
        //    }
        //    else if (prop.PropertyType == typeof(Nullable<DateTime>))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, null);
        //    }
        //    else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long) || prop.PropertyType == typeof(double))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue( obj, cursor.GetStringValue(source).GetIntValue() );
        //            //prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, 0);
        //    }
        //    else if (prop.PropertyType == typeof(Nullable<int>) || prop.PropertyType == typeof(Nullable<long>) || prop.PropertyType == typeof(Nullable<double>))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, null);
        //    }
        //    else if (prop.PropertyType == typeof(bool))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, false);
        //    }
        //    else if (prop.PropertyType == typeof(Nullable<bool>))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, null);
        //    }
        //    else if (prop.PropertyType == typeof(Guid))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, Guid.Empty);
        //    }
        //    else if (prop.PropertyType == typeof(Nullable<Guid>))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, null);
        //    }
        //    else if (prop.PropertyType == typeof(DateTimeOffset))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, DateTimeOffset.MinValue);
        //    }
        //    else if (prop.PropertyType == typeof(Nullable<DateTimeOffset>))
        //    {
        //        if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
        //            prop.SetValue(obj, cursor[source]);
        //        else
        //            prop.SetValue(obj, null);
        //    }
        //    else if (prop.PropertyType.IsEnum)
        //    {
        //        Type t = prop.PropertyType;

        //        // convert to a string value
        //        string val = cursor.GetStringValue(source);

        //        // upcast the string
        //        Object temp = Enum.Parse(t, val, true);

        //        //prop.SetValue( obj, temp );

        //        // if the parsed value is not null then act normal
        //        if (temp != null)
        //        {
        //            prop.SetValue(obj, temp);
        //        }
        //        else // if the enum type is nullable, then set to null else set to default(T)
        //        {
        //            if (t == typeof(Nullable<>))
        //                prop.SetValue(obj, null);
        //            else
        //                prop.SetValue(obj, Activator.CreateInstance(t));
        //        }
        //    }
        //    else
        //    {
        //        throw new System.Exception("Type not yet supported.");
        //    }
        //}

        #endregion [Methods]
    }
}
