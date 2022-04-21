using System;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Logging
{
    /// <summary>Logs to Azure Insights</summary>
    /// <remarks>Included only as a reference</remarks>
    //public class InsightsLogger : Raydreams.Common.Logging.ILogger
    //{
    //    #region [ Fields ]

    //    private Common.Logging.LogLevel _level = Common.Logging.LogLevel.All;

    //    private string _src = null;

    //    #endregion [ Fields ]

    //    public InsightsLogger( ILogger logger )
    //    {
    //        this.Logger = logger;
    //    }

    //    public Common.Logging.LogLevel Level
    //    {
    //        get { return this._level; }
    //        set { this._level = value; }
    //    }

    //    protected Microsoft.Extensions.Logging.ILogger Logger { get; set; }

    //    protected Microsoft.Extensions.Logging.LogLevel GetLevel( Common.Logging.LogLevel lvl )
    //    {
    //        switch ( lvl )
    //        {
    //            case Common.Logging.LogLevel.All:
    //            case Common.Logging.LogLevel.Trace:
    //                return LogLevel.Trace;
    //            case Common.Logging.LogLevel.Debug:
    //                return LogLevel.Debug;
    //            case Common.Logging.LogLevel.Test:
    //            case Common.Logging.LogLevel.Info:
    //                return LogLevel.Information;
    //            case Common.Logging.LogLevel.Warn:
    //                return LogLevel.Warning;
    //            case Common.Logging.LogLevel.Error:
    //                return LogLevel.Error;
    //            case Common.Logging.LogLevel.Fatal:
    //                return LogLevel.Critical;
    //            case Common.Logging.LogLevel.Off:
    //                return LogLevel.None;
    //            default:
    //                return LogLevel.Information;
    //        }
    //    }

    //    public void Debug( string message )
    //    {
    //        this.Logger.LogDebug( message );
    //    }

    //    public void Log( string message, Common.Logging.LogLevel level = Common.Logging.LogLevel.Info )
    //    {
    //        this.InsertLog( level, null, message, null );
    //    }

    //    public void Log( string message, string category, Common.Logging.LogLevel level = Common.Logging.LogLevel.Info )
    //    {
    //        this.InsertLog( level, category, message, null );
    //    }

    //    public void Log( string message, string category, Common.Logging.LogLevel level, params object[] args )
    //    {
    //        this.InsertLog( level, category, message, args );
    //    }

    //    public void Log( Exception exp )
    //    {
    //        this.InsertLog( Common.Logging.LogLevel.Error, null, exp.ToLogMsg( true ), null );
    //    }

    //    public void Log( Exception exp, params object[] args )
    //    {
    //        this.InsertLog( Common.Logging.LogLevel.Error, null, exp.ToLogMsg( true ), args );
    //    }

    //    protected void InsertLog( Common.Logging.LogLevel lvl, string category, string msg, params object[] args )
    //    {
    //        this.Logger.Log( this.GetLevel( lvl ), msg, args );
    //    }
    //}
}
