namespace Raydreams.Common.Model
{
	/// <summary>Status Result codes for API calls</summary>
	/// <remarks>This is an enumeration of everything I use for now. The list only grows.</remarks>
	public enum APIResultType
	{
		/// <summary>Successful results. Carry on.</summary>
		Success = 0,
		/// <summary>First time the user has ever logged in so they may need to set their UserID and Password.</summary>
		FirstLogin = 1,
		/// <summary>The user has to reset their password before they do anything.</summary>
		ResetPassword = 2,
		/// <summary>The user's account is disabled either by an admin or because of too many failed login attempts</summary>
		Disabled = 3,
		/// <summary>The user ID is already taken</summary>
		UserIDTaken = 4,
		/// <summary>The email address is already in use</summary>
		EmailTaken = 5,
		/// <summary>Invalid PW format</summary>
		InvalidPWFormat = 6,
		/// <summary>Invalid User ID format</summary>
		InvalidUserIDFormat = 7,
		/// <summary>Invalid email format</summary>
		InvalidEmailFormat = 8,
		/// <summary>The credentials are invalid or the account is terminated. Return an unauthorzied.</summary>
		InvalidCredentials = 10,
		/// <summary>Password does not match the one in the DB</summary>
		IncorrectPW = 11,
		/// <summary>There's no matching UserID in the DB</summary>
		UserIDNotFound = 12,
		/// <summary>The user has no role in this domain or the domain does not exist</summary>
		InvalidDomain = 13,
		/// <summary>There's no matching code or ID found in the DB</summary>
		CodeNotFound = 14,
		/// <summary>The input parameters do not validate</summary>
		InvalidInput = 21,
		/// <summary>No results data was obtained to return</summary>
		NoResults = 22,
		/// <summary>Failed a logic rule, see the logs or additional data</summary>
		FailedRule = 23,
		/// <summary>No data matching the search or filter</summary>
		NoMatch = 24,
		/// <summary>The user role is not authorized to access this method.</summary>
		Unauthorized = 31,
		/// <summary>The user has maxed out their usage of this method.</summary>
		ExceededQuota = 32,
		/// <summary>User must be logged in to use this method in this way.</summary>
		IllegalAnonymousRequest = 33,
		/// <summary>The app is purposely offline. No one is allowed to login.</summary>
		Offline = 90,
		/// <summary>A backend server is is not responding.</summary>
		Down = 98,
		/// <summary>An exception was thrown, see the logs</summary>
		Exception = 99,
		/// <summary>Error unknown, check the logs</summary>
		Unknown = 100
	}
}
