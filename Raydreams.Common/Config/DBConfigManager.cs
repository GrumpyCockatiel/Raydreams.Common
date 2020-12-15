using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Raydreams.Common.Config
{
	/// <summary>Setting Manager handles loading all settings from a standard SQL DB table into a dictionary.</summary>
	/// <remarks>You must explicitly call LoadAll or another load method. Refactor to use Setting Repository</remarks>
	public class DBConfigManager : IEnumerable<DBConfigValue>
	{
		#region [ Fields ]
		private static readonly string _selectAllSettings = "SELECT * FROM {0} WHERE AppKey = @appKey";
		private static readonly string _updateSetting = "UPDATE {0} SET [Value] = @value WHERE AppKey = @appKey AND SettingKey = @setKey";

		private SqlConnection _dbConn = null;
		private string _appKey = null;
		private string _setTable = null;
		private Dictionary<string, DBConfigValue> _settings = null;

		#endregion [ Fields ]

		#region [ Constructor ]

		/// <summary></summary>
		public DBConfigManager(string appKey, string connStr, string setTable)
		{
			if (!String.IsNullOrWhiteSpace(appKey))
				this._appKey = appKey.Trim();

			if (!String.IsNullOrWhiteSpace(setTable))
				this._setTable = setTable.Trim();

			this._dbConn = new SqlConnection(connStr);
		}

		#endregion [ Constructor ]

		#region [ Properties ]

		/// <summary></summary>
		public SqlConnection DBConnection
		{
			get { return this._dbConn; }
		}

		/// <summary>
		/// The number of values that are loaded.
		/// </summary>
		public int Length
		{
			get { return this._settings.Count; }
		}

		/// <summary>Get a specific settings value.</summary>
		public DBConfigValue this[string key]
		{
			get
			{
				if (String.IsNullOrWhiteSpace(key) || !this._settings.ContainsKey(key))
					return null;

				return this._settings[key];
			}
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary></summary>
		/// <returns></returns>
		public IEnumerator<DBConfigValue> GetEnumerator()
		{
			foreach ( KeyValuePair<string, DBConfigValue> kvp in this._settings )
			{
				yield return kvp.Value;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator();
		}

		/// <summary>Returns just a specific settings value or null</summary>
		public bool HasValue(string key)
		{
			if (String.IsNullOrWhiteSpace(key) || !this._settings.ContainsKey(key))
				return false;

			if (this[key] == null || String.IsNullOrWhiteSpace(this[key].Value))
				return false;

			return true;
		}

		/// <summary>Returns just a specific settings value or null</summary>
		public string GetValue(string key)
		{
			if (String.IsNullOrWhiteSpace(key) || !this._settings.ContainsKey(key))
				return null;

			if (this[key] == null || String.IsNullOrWhiteSpace(this[key].Value))
				return null;

			return this[key].Value;
		}

		/// <summary>Explictily call to load all the settings into memory.</summary>
		/// <returns>If the load was successful.</returns>
		public bool LoadAll()
		{
			if (String.IsNullOrWhiteSpace(_appKey) || String.IsNullOrWhiteSpace(_setTable) || this.DBConnection == null)
				return false;

			DataTable src = new DataTable("Settings");
			SqlCommand select = new SqlCommand(String.Format(_selectAllSettings, this._setTable), this.DBConnection);
			select.Parameters.Add("@appKey", SqlDbType.VarChar, 31).Value = this._appKey;
			SqlDataAdapter da = new SqlDataAdapter(select);

			try
			{
				da.Fill(src);

				this._settings = new Dictionary<string, DBConfigValue>();

				foreach (DataRow dr in src.Rows)
				{
					DBConfigValue set = new DBConfigValue();
					set.Key = dr["SettingKey"].ToString();
					set.DataType = Type.GetType(dr["DataType"].ToString());
					set.IsNull = (dr["Value"] == Convert.DBNull);
					set.Value = dr["Value"].ToString();

					this._settings.Add(set.Key, set);
				}
			}
			catch (System.Data.SqlClient.SqlException)
			{
				return false;
			}
			catch (System.Exception exp)
			{
				throw exp;
			}

			return true;
		}

		/// <summary>Updates a specific setting.</summary>
		/// <param name="newValue"></param>
		/// <returns></returns>
		public int UpdateSetting(string settingKey, string newValue)
		{
			int rows = 0;

			if (String.IsNullOrWhiteSpace(settingKey) && String.IsNullOrWhiteSpace(newValue))
				return rows;

			SqlCommand insert = new SqlCommand(String.Format(_updateSetting, this._setTable), this.DBConnection);
			insert.Parameters.Add("@value", SqlDbType.VarChar, 1023).Value = newValue.Trim();
			insert.Parameters.Add("@appKey", SqlDbType.VarChar, 31).Value = this._appKey;
			insert.Parameters.Add("@setKey", SqlDbType.VarChar, 31).Value = settingKey.Trim();

			try
			{
				this.DBConnection.Open();
				rows = insert.ExecuteNonQuery();
			}
			catch (System.Exception exp)
			{
				throw exp;
			}
			finally
			{
				if (this.DBConnection.State != ConnectionState.Closed)
					this.DBConnection.Close();
			}

			return rows;
		}

		#endregion [ Methods ]

	}
}
