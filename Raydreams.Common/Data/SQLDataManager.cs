using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Raydreams.Common.Data
{
	/// <summary>Generic SQL DB Manager class to perform abstract Select and Excute statements. Create classes with FieldSource attributes to do automatic mapping.</summary>
	/// <remarks>You should always subclass this class. Automatic mapping for Updates and Inserts is not yet supporte so you will need to add those methods to the derives class and call Execute in this class. See the LogRepository for a basic example.</remarks>
	public abstract class SQLDataManager
	{
		#region [Fields]

		private static readonly string _dbInfo = @"SELECT SERVERPROPERTY('productversion') AS [ProductVersion], SERVERPROPERTY('productlevel') AS [ProductLevel], SERVERPROPERTY('edition') AS [Edition], SERVERPROPERTY('EditionID') AS [EditionID], SERVERPROPERTY('ServerName') AS [MachineName]";

		private static readonly string _truncateTable = "TRUNCATE TABLE {0}";
		private static readonly string _backupTable = "INSERT INTO {0} SELECT * FROM {1}";

		private SqlConnection _dbConn = null;

		#endregion [Fields]

		#region [Constructors]

		/// <summary>Defers setting the connection string to later in derived classes</summary>
		protected SQLDataManager()
		{
		}

		/// <summary></summary>
		public SQLDataManager(string connStr)
		{
			this._dbConn = new SqlConnection(connStr);
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary></summary>
		public SqlConnection DBConnection
		{
			get { return this._dbConn; }
			protected set { this._dbConn = value; }
		}

		#endregion [Properties]

		#region [ Converters ]

		/// <summary>Validates a string to a DB value.</summary>
		/// <param name="nullable">true if the result can be DB Null, otherwise false if the DB value cannot be null</param>
		protected object GetDBValue(string s, bool nullable = true)
		{
			if (!nullable)
				return (String.IsNullOrWhiteSpace(s)) ? String.Empty : s.Trim();

			return (String.IsNullOrWhiteSpace(s)) ? Convert.DBNull : s.Trim();
		}

		/// <summary>Validates a DateTime to a DB value</summary>
		protected object GetDBValue(DateTime? d, bool nullable = true)
		{
			if (!nullable)
				return (d == null) ? DateTime.Now : d;

			return (d == null) ? Convert.DBNull : d;
		}

		/// <summary>Validates a DateTime to a DB value</summary>
		protected object GetDBValue(int? i, bool nullable = true)
		{
			if (!nullable)
				return (i == null) ? 0 : i;

			return (i == null) ? Convert.DBNull : i;
		}

		/// <summary>Validates a DateTimeOffset to a DB value. Uses Current UTC time if can not be null.</summary>
		protected object GetDBValue(DateTimeOffset? d, bool nullable = true)
		{
			if (!nullable)
				return (d == null) ? new DateTimeOffset(DateTime.UtcNow) : d;

			return (d == null) ? Convert.DBNull : d;
		}

		#endregion [ Converters ]

		#region [Select Methods]

		/// <summary></summary>
		/// <returns></returns>
		public string DBInfo()
		{
			SqlCommand cmd = new SqlCommand(_dbInfo, this.DBConnection);
			List<Dictionary<string, string>> dt = this.Select(cmd);

			if (dt.Count < 1)
				return null;

			string server = dt[0]["MachineName"];
			string ver = dt[0]["ProductVersion"];

			return String.Format("DB Server : {0}; SQL Version : {1}", server, ver);
		}

		/// <summary>Select 2 columns as a key value pair. Useful for drop down menus.</summary>
		/// <param name="cmd"></param>
		/// <param name="keyColName"></param>
		/// <param name="valColName"></param>
		/// <returns></returns>
		/// <remarks>Create a generic version to will conver to the key to type T</remarks>
		protected List<KeyValuePair<string, string>> SelectDictionary(SqlCommand cmd, string keyColName, string valColName)
		{
			List<KeyValuePair<string, string>> results = new List<KeyValuePair<string, string>>();

			if (String.IsNullOrWhiteSpace(keyColName) || String.IsNullOrWhiteSpace(valColName))
				return null;

			SqlDataReader reader = null;

			try
			{
				this.DBConnection.Open();

				reader = cmd.ExecuteReader();

				while (reader.Read())
				{
					string key = reader.GetStringValue(keyColName);

					if (String.IsNullOrWhiteSpace(key))
						continue;

					string value = reader.GetStringValue(valColName);
					results.Add(new KeyValuePair<string, string>(key, value));
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

		/// <summary>Selects everything from the specified table.</summary>
		protected List<T> SelectAll<T>(string tableName, string context = null) where T : new()
		{
			// validate the table name, really need a Regex
			if (String.IsNullOrWhiteSpace(tableName) || tableName.Contains(';'))
				return null;

			string query = String.Format("SELECT * FROM {0}", tableName);
			SqlCommand command = new SqlCommand(query, this.DBConnection);

			return this.Select<T>(command, context);
		}

		/// <summary>Selects using the specified query and maps to a list of object T</summary>
		/// <param name="context">The context to use but can be set to null or empty string to use a default FieldSource. Don't use any FieldSource attributes to map directly from a property name.</param>
		protected List<T> Select<T>(SqlCommand cmd, string context = null) where T : new()
		{
			List<T> results = new List<T>();

			SqlDataReader reader = null;

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

		/// <summary>Selects using the specified query</summary>
		/// <returns>Returns each record as a String, String Dictionary. Consumer will have to parse.</returns>
		protected List<Dictionary<string, string>> Select(SqlCommand cmd)
		{
			List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();

			SqlDataReader reader = null;

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

		/// <summary>Returns a scaler query as an integer.</summary>
		/// <param name="def">default value to use if there is no query match or error</param>
		protected int SelectIntScaler(SqlCommand cmd, int def = 0)
		{
			int value = def;

			try
			{
				this.DBConnection.Open();

				object result = cmd.ExecuteScalar();

				if (result != null)
					value = Convert.ToInt32(result);
			}
			catch (System.Exception exp)
			{
				throw exp;
			}
			finally
			{
				// close the reader and connection
				this.DBConnection.Close();
			}

			return value;
		}

		/// <summary>Returns a scaler query as a string.</summary>
		protected string SelectStringScaler(SqlCommand cmd)
		{
			string value = null;

			try
			{
				this.DBConnection.Open();

				object result = cmd.ExecuteScalar();

				if (result != null)
					value = result.ToString();
			}
			catch (System.Exception exp)
			{
				throw exp;
			}
			finally
			{
				// close the reader and connection
				this.DBConnection.Close();
			}

			return value;
		}

		/// <summary>Returns a scaler query as a boolean.</summary>
		protected bool SelectBoolScaler(SqlCommand cmd)
		{
			bool value = false;

			try
			{
				this.DBConnection.Open();

				object result = cmd.ExecuteScalar();

				value = (result.ToString().Trim().ToLower()[0] == '1');
			}
			catch (System.Exception exp)
			{
				throw exp;
			}
			finally
			{
				// close the reader and connection
				this.DBConnection.Close();
			}

			return value;
		}

		/// <summary>Returns a scaler query as a GUID.</summary>
		protected Guid SelectGuidScaler(SqlCommand cmd)
		{
			Guid value = Guid.Empty;

			try
			{
				this.DBConnection.Open();

				object result = cmd.ExecuteScalar();

				value = new Guid(result.ToString());
			}
			catch (System.Exception exp)
			{
				throw exp;
			}
			finally
			{
				// close the reader and connection
				this.DBConnection.Close();
			}

			return value;
		}

		/// <summary>Converts the first column in the result set to a list of integers</summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		protected List<int> SelectIntList(SqlCommand cmd)
		{
			List<int> results = new List<int>();

			SqlDataReader reader = null;

			try
			{
				this.DBConnection.Open();

				reader = cmd.ExecuteReader();

				if (reader == null || !reader.HasRows)
					return results;

				while (reader.Read())
				{
					if (reader[0] == Convert.DBNull)
						continue;

					// read a new record from the DB
					int n = Convert.ToInt32(reader[0]);

					results.Add(n);
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

		/// <summary>Converts the first column in the result set to a list of strings</summary>
		/// <param name="cmd"></param>
		/// <returns></returns>
		protected List<string> SelectStringList(SqlCommand cmd)
		{
			List<string> results = new List<string>();

			SqlDataReader reader = null;

			try
			{
				this.DBConnection.Open();

				reader = cmd.ExecuteReader();

				if (reader == null || !reader.HasRows)
					return results;

				while (reader.Read())
				{
					if (reader[0] == Convert.DBNull)
						continue;

					// read a new record from the DB
					string n = reader[0].ToString();

					if (String.IsNullOrWhiteSpace(n))
						continue;

					results.Add(n);
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

		#endregion [Select Methods]

		/// <summary>Insert the generic object into the specified database table where Property Names exactyl match the field names.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="tableName"></param>
		/// <returns></returns>
		/// <remarks>Right now casts enums to an Int until we have some way to indicate how to handle them</remarks>
		protected int Insert<T>(T obj, string tableName, string context = null) where T : new()
		{

			if (String.IsNullOrWhiteSpace(tableName))
				return 0;

			List<string> propNames = new List<string>();
			List<string> values = new List<string>();

			// get all the properties in the class
			PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if (props.Length < 1)
				return 0;

			// set the value of each property on the object
			foreach (PropertyInfo prop in props)
			{
				// can you publicly read the property
				if (!prop.CanRead)
					continue;

				// reject classes except strings
				if (prop.PropertyType.IsClass && !QuotableType(prop.PropertyType))
					continue;

				// reject arrays and collections
				if (prop.PropertyType.IsArray || (prop.PropertyType.IsGenericType && prop.PropertyType.GetGenericTypeDefinition() == typeof(List<>)))
					continue;

				propNames.Add(prop.Name);

				object value = prop.GetValue(obj);

				if (value == null)
				{ values.Add("NULL"); }
				else if (QuotableType(prop.PropertyType))
				{
					values.Add(String.Format("'{0}'", value.ToString()));
				}
				else if (prop.PropertyType == typeof(bool) || prop.PropertyType == typeof(Nullable<bool>))
				{
					values.Add(((bool)value) ? "1" : "0");
				}
				else if (prop.PropertyType.IsEnum)
				{
					values.Add(String.Format("{0}", (int)value));
				}
				else
					values.Add(String.Format("{0}", value.ToString()));
			}

			string query = String.Format("INSERT INTO {0} ({1}) VALUES ({2})", tableName, String.Join(",", propNames), String.Join(",", values));

			SqlCommand cmd = new SqlCommand(query, this.DBConnection);

			return this.Execute(cmd);
		}

		/// <summary></summary>
		protected int Execute(SqlCommand cmd)
		{
			int rows = 0;

			try
			{
				this.DBConnection.Open();

				rows = cmd.ExecuteNonQuery();

			}
			catch (System.Exception exp)
			{
				throw exp;
			}
			finally
			{
				// close the reader and connection
				this.DBConnection.Close();
			}

			return rows;
		}

		#region [Private Methods]

		/// <summary>Parses to an exact matching property name.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cursor"></param>
		/// <returns></returns>
		private T ParseRecord<T>(SqlDataReader cursor) where T : new()
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

				this.Parse<T>(obj, prop, cursor, source);
			}

			return obj;
		}

		/// <summary>Parses to FieldSource adorned attributes.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="cursor"></param>
		/// <param name="context">The context to use to parse, can be null and will use the first FieldSource attribute with no context.</param>
		/// <returns></returns>
		private T ParseRecord<T>(SqlDataReader cursor, string context = null) where T : new()
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

				// if there is not field map source, then this property is not read from the CSV
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

				this.Parse<T>(obj, prop, cursor, map.Source);
			}

			return obj;
		}

		/// <summary>Does the actual conversion</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="obj"></param>
		/// <param name="prop"></param>
		/// <param name="cursor"></param>
		/// <param name="source"></param>
		private void Parse<T>(T obj, PropertyInfo prop, SqlDataReader cursor, string source)
		{
			// use the source field to get the value from the dictionary
			if (prop.PropertyType == typeof(string))
			{
				prop.SetValue(obj, cursor.GetStringValue(source));
			}
			else if (prop.PropertyType == typeof(DateTime))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, DateTime.MinValue);
			}
			else if (prop.PropertyType == typeof(Nullable<DateTime>))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, null);
			}
			else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(long) || prop.PropertyType == typeof(double))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, 0);
			}
			else if (prop.PropertyType == typeof(Nullable<int>) || prop.PropertyType == typeof(Nullable<long>) || prop.PropertyType == typeof(Nullable<double>))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, null);
			}
			else if (prop.PropertyType == typeof(bool))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, false);
			}
			else if (prop.PropertyType == typeof(Nullable<bool>))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, null);
			}
			else if (prop.PropertyType == typeof(Guid))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, Guid.Empty);
			}
			else if (prop.PropertyType == typeof(Nullable<Guid>))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, null);
			}
			else if (prop.PropertyType == typeof(DateTimeOffset))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, DateTimeOffset.MinValue);
			}
			else if (prop.PropertyType == typeof(Nullable<DateTimeOffset>))
			{
				if (cursor.HasColumn(source, false) && cursor[source] != Convert.DBNull)
					prop.SetValue(obj, cursor[source]);
				else
					prop.SetValue(obj, null);
			}
			else if (prop.PropertyType.IsEnum)
			{
				Type t = prop.PropertyType;

				// convert to a string value
				string val = cursor.GetStringValue(source);

				// upcast the string
				Object temp = Enum.Parse(t, val, true);

				//prop.SetValue( obj, temp );

				// if the parsed value is not null then act normal
				if (temp != null)
				{
					prop.SetValue(obj, temp);
				}
				else // if the enum type is nullable, then set to null else set to default(T)
				{
					if (t == typeof(Nullable<>))
						prop.SetValue(obj, null);
					else
						prop.SetValue(obj, Activator.CreateInstance(t));
				}
			}
			else
			{
				throw new System.Exception("Type not yet supported.");
			}
		}

		/// <summary></summary>
		/// <param name="t"></param>
		/// <returns></returns>
		private static bool QuotableType(Type t)
		{
			if (t == typeof(string))
				return true;
			else if (t == typeof(DateTime) || t == typeof(Nullable<DateTime>))
				return true;
			else if (t == typeof(DateTimeOffset) || t == typeof(Nullable<DateTimeOffset>))
				return true;
			else if (t == typeof(Guid) || t == typeof(Nullable<Guid>))
				return true;

			return false;
		}

		#endregion [Private Methods]

		#region [Utility Methods]

		/// <summary>Searches a query and replaces all the table name tokens with the corresponding property string.</summary>
		/// <returns></returns>
		/// <remarks>
		/// SELECT * FROM {{MyTable}}
		/// {{MyTable}} will be replaced by the class property with the name MyTableName. Observe that 'Name' is left off the token.
		/// try to make it work with MyViewName as well
		/// </remarks>
		protected string ReplaceTableNames(string query)
		{
			if (String.IsNullOrWhiteSpace(query))
				return String.Empty;

			PropertyInfo[] props = this.GetType().GetProperties();

			Regex pattern = new Regex(@"[{]{2}([a-zA-Z]+)[}]{2}", RegexOptions.IgnoreCase);

			MatchCollection mc = pattern.Matches(query);

			foreach (Match m in mc)
			{
				// get the table name
				string tableName = m.Groups[1].Value;

				// find a property with the name
				PropertyInfo prop = props.Where(p => p.Name == String.Format("{0}Name", tableName)).FirstOrDefault();

				if (prop == null || !prop.CanRead || prop.PropertyType != typeof(string))
					continue;

				// get the value name
				string replace = prop.GetValue(this).ToString();

				if (String.IsNullOrWhiteSpace(replace))
					continue;

				// replace the token
				query = query.Replace(String.Format("{{{{{0}}}}}", tableName), replace);
			}

			return query;
		}

		/// <summary>Trncates the destination table.</summary>
		/// <param name="src">Tabel to copy from</param>
		/// <param name="dest">Table to truncate and copy to</param>
		protected virtual int BackupTable(string srcTable, string bkupTable)
		{
			if (String.IsNullOrWhiteSpace(srcTable) || String.IsNullOrWhiteSpace(bkupTable))
				return 0;

			// should check the fields all match

			// first truncate the dest
			string q1 = String.Format(_truncateTable, bkupTable);
			SqlCommand cmd1 = new SqlCommand(q1, this.DBConnection);

			this.Execute(cmd1);

			// now copy
			string q2 = String.Format(_backupTable, bkupTable, srcTable);


			SqlCommand cmd2 = new SqlCommand(q2, this.DBConnection);

			return this.Execute(cmd2);
		}

		/// <summary>Truncates the entire contents of a table, be careful using this.</summary>
		protected virtual int TruncateTable(string tableName)
		{
			if (String.IsNullOrWhiteSpace(tableName))
				return 0;

			string q = String.Format(_truncateTable, tableName);
			SqlCommand cmd = new SqlCommand(q, this.DBConnection);

			try
			{
				return this.Execute(cmd);
			}
			catch
			{
				return 0;
			}
		}

		#endregion [Utility Methods]
	}
}
