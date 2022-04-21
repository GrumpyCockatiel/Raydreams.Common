using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Driver;
using Raydreams.Common.Extensions;
using System.Threading.Tasks;
using Raydreams.Common.Data;

namespace Raydreams.Common.Logging
{
	/// <summary>Logs to a Mongo DB Collection usually named Logs</summary>
	/// <remarks>This class is pretty old and could probably use a review and cleanup</remarks>
	public class MongoLogger : MongoDataManager<LogRecord, long>, ILogRepository, ILogger
	{
		#region [Fields]

		private string _table = "Logs";

		private LogLevel _level = LogLevel.Off;

		private string _src = null;

		#endregion [Fields]

		#region [Constructor]

		/// <summary>Constructor</summary>
		/// <param name="connStr"></param>
		/// <param name="db"></param>
		/// <param name="table"></param>
		/// <param name="src"></param>
		public MongoLogger( string connStr, string db, string table, string src = null ) : base( connStr, db )
		{
			this.TableName = table;
			this.Source = src;
		}

		#endregion [Constructor]

		#region [Properties]

		/// <summary>The maximum number of logs to return</summary>
		public int Max { get; set; } = 200;

		/// <summary>The name of the logging table or collection</summary>
		public string TableName
		{
			get { return this._table; }
			set
			{
				if ( !String.IsNullOrWhiteSpace( value ) )
					this._table = value.Trim();
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

		#region [ Base Methods ]

		/// <summary>Set the minimum level to log</summary>
		/// <param name="lvl"></param>
		/// <returns></returns>
		public MongoLogger SetLevel( LogLevel lvl )
		{
			this.Level = lvl;
			return this;
		}

		/// <summary>Sample method that just gets all the logs</summary>
		/// <remarks>Generally do not want to use this</remarks>
		public List<LogRecord> List()
		{
			return base.GetAll( this.TableName );
		}

		/// <summary>Gets only the top N logs sorted descending by timestamp</summary>
		/// <returns></returns>
		public List<LogRecord> ListTop( int top = 100 )
		{
			if ( top > this.Max )
				top = this.Max;

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			List<LogRecord> results = collection.Find( FilterDefinition<LogRecord>.Empty ).Sort( "{timestamp: -1}" ).Limit( top ).ToList();

			return ( results != null && results.Count > 0 ) ? results : new List<LogRecord>();
		}

		/// <summary>Get logs only on a single day</summary>
		public List<LogRecord> ListByDay( DateTimeOffset day )
		{
			return this.ListByRange( day.StartOfDay(), day.EndOfDay() );
		}

		/// <summary>Gets logs within a date range</summary>
		/// <returns></returns>
		public List<LogRecord> ListByRange( DateTimeOffset begin, DateTimeOffset end )
		{
			if ( begin > end )
				return new List<LogRecord>();

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			return collection.Find<LogRecord>( t => t.Timestamp >= begin && t.Timestamp < end ).ToList();
		}

		/// <summary>Deletes any logs older than the specified number of days</summary>
		/// <param name="days">Number of days</param>
		/// <returns>Records removed</returns>
		public async Task<long> PurgeAfter( int days = 90 )
		{
			if ( days < 0 )
				return 0;

			DateTime expire = DateTime.UtcNow.Subtract( new TimeSpan( days, 0, 0, 0 ) );

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			DeleteResult results = await collection.DeleteManyAsync<LogRecord>( t => t.Timestamp < expire );

			return ( results.IsAcknowledged ) ? results.DeletedCount : 0;
		}

		#endregion [ Base Methods ]

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

		/// <summary>Base logging method</summary>
		/// <param name="logger">The source of the source such as the application name.</param>
		/// <param name="lvl">The std log level as defined by Log4j</param>
		/// <param name="category">An application specific category that can be used for further organization, or routing to differnt locations/</param>
		/// <param name="msg">The actual message to log</param>
		/// <param name="args">any additional data fields to append to the log message. Used for debugging.</param>
		/// <returns></returns>
		protected bool InsertLog( string logger, LogLevel lvl, string category, string msg, params object[] args )
		{
			if ( lvl < this.Level )
				return false;

			try
			{
				if ( String.IsNullOrWhiteSpace( logger ) )
					logger = Assembly.GetExecutingAssembly().FullName;
				else
					logger = logger.Trim();

				// convert the args dictionary to a string and add to the end
				//if ( args != null && args.Length > 0 )
				//msg = String.Format( "{0} args={1}", msg, String.Join( ";", args ) );

				if ( String.IsNullOrWhiteSpace( msg ) )
					msg = String.Empty;

				// inserts are fire and forget
				_ = base.Insert( new LogRecord( msg, lvl )
				{ Category = category, Source = logger, Args = args },
					this.TableName );
			}
			catch ( System.Exception exp )
			{
				throw exp;
			}

			return true;
		}

		#endregion [ ILogger ]
	}
}
