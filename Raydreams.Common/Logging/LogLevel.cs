namespace Raydreams.Common.Logging
{
	/// <summary>Standard Log levels</summary>
	public enum LogLevel
	{
		/// <summary>Turns on All Logging</summary>
		All = -1,
		/// <summary>very detailed logs, which may include high-volume information such as protocol payloads. This log level is typically only enabled during development</summary>
		Trace = 0,
		/// <summary>debugging information, less detailed than trace, typically not enabled in production environment.</summary>
		Debug = 1,
		/// <summary>Used to test the logger.</summary>
		Test = 2,
		/// <summary>information messages, which are normally enabled in production environment</summary>
		Info = 3,
		/// <summary>warning messages, typically for non-critical issues, which can be recovered or which are temporary failures</summary>
		Warn = 4,
		/// <summary>error messages</summary>
		Error = 5,
		/// <summary>Critical Errors that need to be addressed immediately</summary>
		Fatal = 6,
		/// <summary>Turns off logging</summary>
		Off = 99
	}
}
