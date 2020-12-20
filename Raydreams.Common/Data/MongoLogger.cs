using System;
using System.Collections.Generic;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Driver;
using Raydreams.Common.Extensions;
using Raydreams.Common.Logging;
using Raydreams.Common.Model;

namespace Raydreams.Common.Data
{
	/// <summary>Logs to a Mongo DB</summary>
	public class MongoLogger : MongoDataManager<LogRecord, long> , ILogger
	{
		#region [Fields]

		private string _table = "Logs";

		private LogLevel _level = LogLevel.Off;

		private string _src = null;

		#endregion [Fields]

		/// <summary></summary>
        /// <param name="connStr"></param>
        /// <param name="db"></param>
        /// <param name="table"></param>
        /// <param name="src"></param>
		public MongoLogger( string connStr, string db, string table, string src = null ) : base( connStr, db )
		{
			//if ( !String.IsNullOrWhiteSpace( connStr ) )
				//this._dbConn = new MongoClient( connStr );

			this.TableName = table;
			this.Source = src;
		}

		#region [Properties]

		/// <summary></summary>
		public int Max { get; set; } = 100;

		/// <summary>The name of the logging table</summary>
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

		/// <summary>Sample method that just gets all the logs</summary>
		public List<LogRecord> GetAll()
		{
			return base.GetAll( this.TableName );
		}

		/// <summary>Gets only transactions for this day right now</summary>
		/// <returns></returns>
		public List<LogRecord> GetTop( int top = 100 )
		{
			if ( top > this.Max )
				top = this.Max;

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			List<LogRecord> results = collection.Find( FilterDefinition<LogRecord>.Empty ).Sort( "{timestamp: -1}" ).Limit( top ).ToList();

			return ( results != null && results.Count > 0 ) ? results : new List<LogRecord>();
		}

		/// <summary>Get logs on a single day</summary>
		public List<LogRecord> GetByDay( DateTimeOffset day )
		{
			return this.GetByDates( day, day );
		}

		/// <summary>Gets only transactions for this day right now</summary>
		/// <returns></returns>
		public List<LogRecord> GetByDates( DateTimeOffset begin, DateTimeOffset end )
		{
			// normaliza the dates
			DateTime start = begin.UtcDateTime.StartOfDay( DateTimeKind.Utc );
			DateTime stop = ( end.UtcDateTime + new TimeSpan( 1, 0, 0, 0 ) ).StartOfDay( DateTimeKind.Utc );

			List<LogRecord> results = new List<LogRecord>();

			if ( start >= stop )
				return results;

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			return collection.Find<LogRecord>( t => t.Timestamp >= start && t.Timestamp < stop ).ToList();
		}

		/// <summary>Deletes any logs older than the specified number of days</summary>
		/// <param name="days">Number of days</param>
		/// <returns>Records removed</returns>
		public long PurgeAfter( int days = 90 )
		{
			if ( days < 0 )
				return 0;

			DateTime expire = DateTime.UtcNow.Subtract( new TimeSpan( days, 0, 0, 0 ) );

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.TableName );
			DeleteResult results = collection.DeleteMany<LogRecord>( t => t.Timestamp < expire );

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
		public void Log(LogRecord message )
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
		protected int InsertLog( string logger, LogLevel lvl, string category, string msg, params object[] args )
		{
			int rows = 0;

			if ( lvl < this.Level )
				return rows;

			if ( String.IsNullOrWhiteSpace( logger ) )
				logger = Assembly.GetExecutingAssembly().FullName;

			// convert the args dictionary to a string and add to the end
			if ( args != null && args.Length > 0 )
				msg = String.Format( "{0} args={1}", msg, String.Join( ";", args ) );

			if ( String.IsNullOrWhiteSpace( msg ) )
				msg = String.Empty;

			try
			{
				BsonDocument log = null;

				if ( String.IsNullOrWhiteSpace( category ) )
					log = new BsonDocument {
					{ "timestamp", DateTime.UtcNow },
					{ "source", logger.Trim() },
					{ "level", lvl.ToString() },
					{ "message", msg }
					};
				else
					log = new BsonDocument {
					{ "timestamp", DateTime.UtcNow },
					{ "source", logger.Trim() },
					{ "level", lvl.ToString() },
					{ "category", category },
					{ "message", msg }
					};

				//IMongoDatabase db = this.Client.GetDatabase( this.Database );
				IMongoCollection<BsonDocument> collection = this.Database.GetCollection<BsonDocument>( this.TableName );
				collection.InsertOne( log );
			}
			catch ( System.Exception exp )
			{
				throw exp;
			}

			return rows;
		}

		#endregion [ ILogger ]
	}
}
