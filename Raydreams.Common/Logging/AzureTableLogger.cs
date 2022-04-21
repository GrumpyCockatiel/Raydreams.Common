using System;
using System.Collections.Generic;
using Raydreams.Common.Extensions;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Raydreams.Common.Data;

namespace Raydreams.Common.Logging
{
	/// <summary>Logs to an Azure Table</summary>
	public class AzureTableLogger : AzureTableRepository<LogRecord>, ILogRepository, ILogger
	{
		private LogLevel _level = LogLevel.All;

		private string _src = null;

		#region [Constructors]

		/// <summary>Constructor with a hard coded table name</summary>
		public AzureTableLogger( string connStr, string src = null ) : base( connStr, "Logs" )
		{
			this.Source = src;
		}

		#endregion [Constructors]

		#region [Properties]

		/// <summary>The minimum level to log which defaults to All</summary>
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

		/// <summary></summary>
		/// <param name="top"></param>
		/// <returns></returns>
		public List<LogRecord> ListTop( int top = 100 )
		{
			throw new NotImplementedException();
		}

		/// <summary></summary>
		/// <param name="top"></param>
		/// <returns></returns>
		public List<LogRecord> ListByRange( DateTimeOffset begin, DateTimeOffset end )
		{
			throw new NotImplementedException();
		}

		/// <summary>Deletes any logs older than the specified number of days</summary>
		/// <param name="days">Age of an old record in days this days = 30 removes all record 30+ days old</param>
		/// <returns>Records removed which will max out at 1000 for now</returns>
		/// <remarks>The returned value has not been tested</remarks>
		public async Task<long> PurgeAfter( int days = 7 )
		{
			if ( days < 0 )
				return 0;

			// subtract days from today
			DateTimeOffset expire = DateTimeOffset.UtcNow.Subtract( new TimeSpan( days, 0, 0, 0 ) );

			TableResult results = null;
			TableQuerySegment<LogRecord> data = null;
			TableContinuationToken tok = new TableContinuationToken();

			// anything with a timestamp less than or equal to today - days is old
			var query = new TableQuery<LogRecord>().Where( TableQuery.GenerateFilterConditionForDate( "Timestamp",
				QueryComparisons.LessThanOrEqual, expire ) );
			data = await this.AzureTable.ExecuteQuerySegmentedAsync<LogRecord>( query, tok );

			foreach ( var row in data )
			{
				var op = TableOperation.Delete( row );
				results = await this.AzureTable.ExecuteAsync( op );
				//results.HttpStatusCode
			}

			return data.Results.Count;
		}

		/// <summary>Removes every record in the logger</summary>
		/// <remarks>This function is very danagerous and should be moved to protected at least</remarks>
		protected void Clear()
		{
			base.DeleteAll();
		}

		#region [ ILogger ]

		/// <summary></summary>
		/// <param name="message"></param>
		public void Debug( string message )
		{
			this.InsertLog( this.Source, LogLevel.Debug, null, message, null );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		public void Log( LogRecord message )
		{
			this.InsertLog( this.Source, message.Level, message.Category, message.Message, message.Args );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		public void Log( string message, LogLevel level = LogLevel.Info )
		{
			this.InsertLog( this.Source, level, null, message, null );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		public void Log( string message, string category, LogLevel level = LogLevel.Info )
		{
			this.InsertLog( this.Source, level, category, message, null );
		}

		/// <summary></summary>
		/// <param name="message"></param>
		/// <param name="level"></param>
		/// <param name="args"></param>
		public void Log( string message, string category, LogLevel level, params object[] args )
		{
			this.InsertLog( this.Source, level, category, message, args );
		}

		/// <summary></summary>
		/// <param name="exp"></param>
		public void Log( System.Exception exp )
		{
			this.InsertLog( this.Source, LogLevel.Error, LogManager.ErrorCategory, exp.ToLogMsg( true ), null );
		}

		/// <summary></summary>
		/// <param name="exp"></param>
		public void Log( Exception exp, params object[] args )
		{
			this.InsertLog( this.Source, LogLevel.Error, LogManager.ErrorCategory, exp.ToLogMsg( true ), args );
		}

		#endregion [ ILogger ]

		/// <summary></summary>
		protected int InsertLog( string logger, LogLevel lvl, string category, string msg, params object[] args )
		{
			if ( lvl < this.Level )
				return 0;

			if ( String.IsNullOrWhiteSpace( logger ) )
				logger = String.Empty;

			if ( String.IsNullOrWhiteSpace( msg ) )
				msg = String.Empty;

			LogRecord rec = new LogRecord
			{
				Source = logger,
				Level = lvl,
				Message = msg,
				Category = category,
				Args = args
			};

			return base.Insert( rec );
		}
	}
}
