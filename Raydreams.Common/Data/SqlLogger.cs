using Raydreams.Common.Data;
using Raydreams.Common.Extensions;
using Raydreams.Common.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace Raydreams.Common.Logging
{
	/// <summary>Logs to a SQL Server DB.</summary>
	public class SqlLogger : SQLDataManager, ILogger
	{
		#region [ Fields ]

		private static readonly string _selectDailyLog = "SELECT * FROM {{Table}} WHERE (Timestamp > DATEADD(day, 0, DATEDIFF(day, 0, GETUTCDATE()))) ORDER BY [Timestamp] DESC";

		private static readonly string _selectLastest = "SELECT TOP 100 * FROM {{Table}} ORDER BY [Timestamp] DESC";

		private static readonly string _cleanse = "DELETE FROM {{Table}} WHERE [Timestamp] < @expire";

		/// <summary>The SQL query to insert a log</summary>
		private static readonly string _insertLog = "INSERT INTO {0} ([Source],[Level],[Category],[Message],[Timestamp]) VALUES (@src,@level,@cat,@msg,@ts)";

		/// <summary></summary>
		private string _src = null;

		/// <summary>Min log level</summary>
		private LogLevel _level = LogLevel.Off;

		/// <summary>Log table name</summary>
		private string _tbl = String.Empty;

		#endregion [ Fields ]

		/// <summary>Constructor</summary>
		/// <param name="src">Application sending the log</param>
		/// <param name="connStr">Connection string</param>
		/// <param name="table">DB table to log to</param>
		public SqlLogger(string source, string connStr, string table = "Logs") : base(connStr)
		{
			this.Source = source;
			this.TableName = table;
		}

		#region [Properties]

		/// <summary>The DB connection</summary>
		//public SqlConnection DBConnection
		//{
		//	get { return this._dbConn; }
		//}

		/// <summary>The name of the logging table</summary>
		public string TableName
		{
			get { return this._tbl; }
			set
			{
				if (!String.IsNullOrWhiteSpace( value ))
					this._tbl = value.Trim().ToLower();
			}
		}

		/// <summary>The minimum level inclusive to log based on the LogLevel enumeration [level,...]</summary>
		public LogLevel Level
		{
			get { return this._level; }
			set { this._level = value; }
		}

		/// <summary>The logging source. Who is doing the logging.</summary>
		public string Source
		{
			get { return this._src; }
			set
			{
				if ( value != null )
					this._src = value.Trim();
			}
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Gets only the last 100 for now</summary>
		/// <returns></returns>
		public List<LogRecord> GetTop( int top = 100 )
		{
			// start with a simple query
			string query = this.ReplaceTableNames( _selectLastest );
			SqlCommand command = new SqlCommand( query, this.DBConnection );

			return this.Select<LogRecord>( command );
		}

		/// <summary></summary>
		public List<LogRecord> GetTodaysLogs()
		{
			// start with a simple query
			string query = this.ReplaceTableNames( _selectDailyLog );
			SqlCommand command = new SqlCommand( query, this.DBConnection );

			return this.Select<LogRecord>( command );
		}

		/// <summary>Deletes log records over some specified number of days</summary>
		/// <param name="days">Defaults to 365.</param>
		/// <returns></returns>
		public int PurgeAfter( int days = 365 )
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

		#region [ ILogger ]

		/// <summary></summary>
		/// <param name="message"></param>
		public void Debug( string message )
		{
			this.InsertLog( this.Source, LogLevel.Debug, null, message, null );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		public void Log(LogRecord message )
		{
			this.InsertLog( this.Source, message.Level, message.Category, message.Message, message.Args );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		public void Log(string message, LogLevel level = LogLevel.Info)
		{
			this.InsertLog( this.Source, level, null, message, null );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		public void Log(string message, string category, LogLevel level = LogLevel.Info)
		{
			this.InsertLog(this.Source, level, category, message, null);
		}

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		/// <param name="args"></param>
		public void Log(string message, string category, LogLevel level, params object[] args)
		{
			this.InsertLog( this.Source, level, category, message, args );
		}

		/// <summary></summary>
		/// <param name="exp"></param>
		public void Log(System.Exception exp)
		{
			this.InsertLog( this.Source, LogLevel.Error, LogManager.ErrorCategory, exp.ToLogMsg( true ), null );
		}

		/// <summary></summary>
		/// <param name="exp"></param>
		public void Log(Exception exp, params object[] args)
		{
			this.InsertLog(this.Source, LogLevel.Error, LogManager.ErrorCategory, exp.ToLogMsg(true), args);
		}

		/// <summary>Base logging method</summary>
		/// <param name="logger">The source of the source such as the application name.</param>
		/// <param name="lvl">The std log level as defined by Log4j</param>
		/// <param name="category">An application specific category that can be used for further organization, or routing to differnt locations/</param>
		/// <param name="msg">The actual message to log</param>
		/// <param name="args">any additional data fields to append to the log message. Used for debugging.</param>
		/// <returns></returns>
		protected int InsertLog(string logger, LogLevel lvl, string category, string msg, params object[] args)
		{
			int rows = 0;

			if (lvl < this.Level)
				return rows;

			if ( String.IsNullOrWhiteSpace( category ) )
				category = null;

			if (String.IsNullOrWhiteSpace(logger))
				logger = Assembly.GetExecutingAssembly().FullName;

			// convert the args dictionary to a string and add to the end
			if ( args != null && args.Length > 0 )
				msg = String.Format("{0} args={1}", msg, String.Join(";", args));

			if (String.IsNullOrWhiteSpace(msg))
				msg = String.Empty;

			SqlCommand insert = new SqlCommand(String.Format(_insertLog, this.TableName), this.DBConnection);
			insert.Parameters.Add( "@src", SqlDbType.VarChar, 127 ).Value = logger.Trim();
			insert.Parameters.Add( "@ts", SqlDbType.DateTimeOffset ).Value = DateTime.UtcNow;
			insert.Parameters.Add( "@level", SqlDbType.VarChar, 15 ).Value = lvl.ToString();
			insert.Parameters.Add( "@msg", SqlDbType.NVarChar ).Value = msg.Trim();

			if ( category == null )
				insert.Parameters.Add( "@cat", SqlDbType.VarChar, 63 ).Value = Convert.DBNull;
			else
				insert.Parameters.Add( "@cat", SqlDbType.VarChar, 63 ).Value = category.Trim();

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

        #endregion [ ILogger ]
    }
}
