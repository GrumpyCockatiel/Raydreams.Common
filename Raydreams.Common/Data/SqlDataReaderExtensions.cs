using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Raydreams.Common.Data
{
	/// <summary></summary>
	public static partial class SqlDataReaderExtensions
	{
		/// <summary>Does the SqlDataReader have a column with the specified column name</summary>
		/// <param name="dr">The data reader</param>
		/// <param name="colName">The name of the column</param>
		/// <param name="ignoreCase">Match on case or not</param>
		/// <returns></returns>
		public static bool HasColumn(this SqlDataReader dr, string colName, bool ignoreCase = false)
		{
			StringComparison options = (ignoreCase) ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

			return Enumerable.Range(0, dr.FieldCount).Any(i => string.Equals(dr.GetName(i), colName, options));
		}

		/// <summary>Extension to SQL Data Reader that convertes the value of any DB field to a string which can be used to parse to its own type.</summary>
		/// <param name="reader"></param>
		/// <param name="colName">Source column to read</param>
		/// <returns>Returns null if no specified source column is found or the value is completely empty</returns>
		public static string GetStringValue(this SqlDataReader reader, string colName)
		{
			//reader.GetSchemaTable().Columns.Contains
			int col = -1;

			try
			{
				col = reader.GetOrdinal(colName);
			}
			catch (IndexOutOfRangeException)
			{
				return null;
			}

			if (col < 0 || reader[col] == Convert.DBNull)
				return null;

			string temp = reader[col].ToString().Trim();

			if (String.IsNullOrWhiteSpace(temp))
				return null;

			return temp;
		}
	}

}
