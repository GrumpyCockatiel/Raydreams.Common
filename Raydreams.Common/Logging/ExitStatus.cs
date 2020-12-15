namespace Raydreams.Common.Logging
{
	/// <summary>Enumerates states in which the application ended.</summary>
	public enum ExitStatus
	{
		/// <summary>Exited normally</summary>
		Success = 0,
		/// <summary>An unspecified failure.</summary>
		GeneralFailure = 1,
		/// <summary>Error loading all settings necessary to run the application.</summary>
		ConfigFailure = 2,
		/// <summary>The user requested help instead of running the app</summary>
		EchoHelp = 3,
		/// <summary>An error occured in some logic of the app but it could continue with later tasks</summary>
		NonFatalFailure = 4,
		/// <summary>No connection to the database.</summary>
		NoDBConnection = 11,
		/// <summary>Some error in the Data Source</summary>
		DataManager = 12
	}
}
