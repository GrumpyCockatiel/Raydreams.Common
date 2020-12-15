using System;
using System.Collections.Generic;
using MongoDB.Driver;
using Raydreams.Common.Extensions;
using Raydreams.Common.Model;

namespace Raydreams.Common.Data
{
	/// <summary>Repository for a SQL based DB logger.</summary>
	public class MongoLogRepository : MongoDataManager<LogRecord,long>
	{
		#region [Fields]

		private string _table = "Logs";

		#endregion [Fields]

		public MongoLogRepository( string connStr, string db, string table ) : base( connStr, db )
		{
			this.Table = table;
		}

		#region [Properties]

		/// <summary></summary>
		public int Max { get; set; }

		/// <summary></summary>
		public string Table
		{
			get { return this._table; }
			protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
		}

		#endregion [Properties]

		#region [Methods]

		/// <summary>Sample method that just gets all the logs</summary>
		public List<LogRecord> GetAll()
		{
			return base.GetAll( this.Table );
		}

		/// <summary>Gets only transactions for this day right now</summary>
		/// <returns></returns>
		public List<LogRecord> GetTop( int top = 100 )
		{
			if ( top > this.Max )
				top = this.Max;

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.Table );
			List<LogRecord> results = collection.Find( FilterDefinition<LogRecord>.Empty ).Sort( "{timestamp: -1}" ).Limit( top ).ToList();

			return (results != null && results.Count > 0) ? results : new List<LogRecord>();
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
			DateTime stop = (end.UtcDateTime + new TimeSpan( 1, 0, 0, 0 )).StartOfDay( DateTimeKind.Utc );

			List<LogRecord> results = new List<LogRecord>();

			if ( start >= stop )
				return results;

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.Table );
			return collection.Find<LogRecord>( t => t.Timestamp >= start && t.Timestamp < stop ).ToList();
		}

		/// <summary>Deletes any logs older than the specified number of days</summary>
		/// <param name="days">Number of days</param>
		/// <returns>Records removed</returns>
		public long PurgeAfter(int days = 90)
		{
			if (days < 0)
				return 0;

			DateTime expire = DateTime.UtcNow.Subtract( new TimeSpan( days, 0, 0, 0 ) );

			IMongoCollection<LogRecord> collection = this.Database.GetCollection<LogRecord>( this.Table );
			DeleteResult results = collection.DeleteMany<LogRecord>( t => t.Timestamp < expire );

			return ( results.IsAcknowledged ) ? results.DeletedCount : 0;
		}

		#endregion [Methods]
	}
}
