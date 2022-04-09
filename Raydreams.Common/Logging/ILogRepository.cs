using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Raydreams.Common.Logging
{
	/// <summary></summary>
	public interface ILogRepository
	{
		/// <summary>list all will be removed</summary>
		//List<LogRecord> List();

		/// <summary>List the top N records</summary>
		List<LogRecord> ListTop( int top = 100 );

		/// <summary>List the top N records</summary>
		List<LogRecord> ListByRange( DateTimeOffset begin, DateTimeOffset end );

		/// <summary></summary>
		Task<long> PurgeAfter( int days );
	}

	/// <summary></summary>
	public class NullLogRepository : ILogRepository
	{
		/// <summary></summary>
		public List<LogRecord> ListTop( int top = 100 )
		{
			return new List<LogRecord>() {
				new LogRecord("The Null Log Repository was loaded!", LogLevel.Error) { Timestamp = DateTimeOffset.UtcNow }
			};
		}

		/// <summary></summary>
		public List<LogRecord> ListByRange( DateTimeOffset begin, DateTimeOffset end )
		{
			return this.ListTop();
		}

		/// <summary></summary>
		public Task<long> PurgeAfter( int days )
		{
			return Task.FromResult( 0L );
		}
	}
}
