﻿using System;
using Raydreams.Common.Model;

namespace Raydreams.Common.Logging
{
	/// <summary>Null Logger to set a logger that does nothing</summary>
	public class NullLogger : ILogger
	{
		#region [ Fields ]
		private LogLevel _level = LogLevel.Off;

        #endregion [ Fields ]

        #region [ Constructors ]

        /// <summary></summary>
        public NullLogger() : this(LogLevel.All)
        { }

        /// <summary></summary>
        /// <param name="baseLevel">Minimum level to log.</param>
        public NullLogger( LogLevel baseLevel )
		{
			this.Level = baseLevel;
		}

		#endregion [ Constructors ]

		#region [ Properties ]

		/// <summary>The minimum level inclusive to log based on the LogLevel enumeration [level,...]</summary>
		public LogLevel Level
		{
			get { return this._level; }
			set { this._level = value; }
		}

		#endregion [ Properties ]

		#region [ Methods ]

		/// <summary></summary>
		public void Debug( string message )
		{
			;
		}

		/// <summary></summary>
		public void Log(LogRecord message )
		{
			;
		}

		/// <summary></summary>
		public void Log( Exception exception )
		{
			;
		}

		/// <summary></summary>
		public void Log( Exception exp, params object[] args )
		{
			;
		}

		/// <summary></summary>
		public void Log( string message, LogLevel level = LogLevel.Info )
		{
			;
		}

		/// <summary></summary>
		public void Log( string message, string category, LogLevel level = LogLevel.Info )
		{
			;
		}

		/// <summary></summary>
		public void Log( string message, string category, LogLevel level, params object[] args )
		{
			;
		}

		#endregion [ Methods ]
	}
}
