using System;
using System.Collections.Specialized;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace Raydreams.Common.Data
{
    /// <summary></summary>
    public static class DbConnectionExtensions
    {
        /// <summary>Tests a SQL Connection to see if it can connect to the DB.</summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static bool TestSqlConnection( this DbConnection conn )
        {
            bool results = false;

            if ( conn == null || String.IsNullOrWhiteSpace( conn.ConnectionString ) )
                return results;

            try
            {
                conn.Open();
            }
            catch
            {
                results = false;
            }

            results = ( conn.State == ConnectionState.Open );

            if ( conn.State != ConnectionState.Closed )
                conn.Close();

            return results;
        }
    }

    /// <summary></summary>
	public static class DataRowExtensions
    {
        /// <summary>Turns a DataRow into a StringDictionary of key value pairs</summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public static StringDictionary DataRowToStringDictionary(this DataRow row)
		{
			StringDictionary sb = new StringDictionary();

			foreach (DataColumn col in row.Table.Columns)
				sb.Add(col.ColumnName, row[col.Ordinal].ToString());

			return sb;
		}

		/// <summary>Simple Table DataRow to property converter. Mathches a column name in the table to a property name. Only works for strings for now, but can be extened to other types.</summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="row"></param>
		/// <returns>Object of type T</returns>
		/// <remarks>This has been added to SQLDataManager with more complexity</remarks>
		public static T ParseDataRow<T>(this DataRow row) where T : new()
		{
			// get all the properties in the class
			PropertyInfo[] props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

			if (props == null || props.Length < 1)
				return default(T);

			T obj = new T();

			foreach (DataColumn col in row.Table.Columns)
			{
				// find a matching property
				PropertyInfo prop = props.Where(p => p.Name == col.ColumnName).FirstOrDefault();

				if (prop == null || !prop.CanWrite)
					continue;

				// convert type
				if (prop.PropertyType == typeof(string))
					prop.SetValue(obj, row.GetStringValue(col.ColumnName));
				else
					throw new System.Exception("Property is not supported yet.");
			}

			return obj;
		}

        /// <summary>Extension to SQL Data Reader to deal with reading string based fields.</summary>
        /// <param name="row"></param>
        /// <param name="colName"></param>
        /// <returns></returns>
        public static string GetStringValue(this DataRow row, string colName)
        {
            if (!row.Table.Columns.Contains(colName) || row.IsNull(colName))
                return null;

            string temp = row[colName].ToString().Trim();

            if (String.IsNullOrWhiteSpace(temp))
                return null;

            return temp;
        }

    }

}
