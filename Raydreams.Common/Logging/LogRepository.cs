using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Raydreams.Common.Data;
using Raydreams.Common.Model;

namespace Raydreams.Common.Logging
{
	/// <summary>Repository for a SQL based DB logger.</summary>
	public class LogRepository : SQLDataManager
	{
		#region [Fields]

		private static readonly string _selectLogs = "SELECT * FROM {{Table}}";

		private static readonly string _cleanse = "DELETE FROM {{Table}} WHERE [Timestamp] < @expire";

		private string _tableName = null;

		#endregion [Fields]

		#region [Constructors]

		/// <summary></summary>
		/// <param name="connStr">The Connection Str to the DB</param>
		public LogRepository( string connStr ) : base(connStr)
		{
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary></summary>
		public string TableName
		{
			get { return this._tableName; }
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._tableName = value.Trim().ToLower();
			}
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Sample method that just gets all the logs</summary>
		public List<LogRecord> GetLogs()
		{
			// start with a simple query
			string query = this.ReplaceTableNames( _selectLogs );
			SqlCommand command = new SqlCommand( query , this.DBConnection );

			return this.Select<LogRecord>( command, null );
		}

		/// <summary></summary>
		/// <param name="days">Number of days after the separation date to close. If less than 0 will not execute.</param>
		/// <returns></returns>
		public int DeleteExpired( int days = 0 )
		{
			if ( days < 0 )
				return 0;

			DateTime now = DateTime.Now.Subtract( new TimeSpan( days, 0, 0, 0 ) );

			// start with a simple query
			string query = this.ReplaceTableNames( _cleanse );
			SqlCommand cmd = new SqlCommand( query, this.DBConnection );
			cmd.Parameters.Add( "@expire", SqlDbType.DateTime2 ).Value = this.GetDBValue( now );

			return this.Execute( cmd );
		}

		#endregion [Methods]
	}
}
