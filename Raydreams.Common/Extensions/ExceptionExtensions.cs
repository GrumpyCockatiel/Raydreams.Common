using System.Text;

namespace Raydreams.Common.Extensions
{
	/// <summary></summary>
	public static class ExceptionExtensions
	{
		/// <summary>Creates a string message from an exception. Extenstion method</summary>
		/// <param name="exp"></param>
		/// <param name="includeStackTrace"></param>
		/// <returns></returns>
		public static string ToLogMsg(this System.Exception exp, bool includeStackTrace = false)
		{
			StringBuilder msg = new StringBuilder();
			msg.AppendFormat( "{0} : {1} ", exp.GetType().FullName, exp.Message );
			if ( includeStackTrace )
				msg.Append( exp.StackTrace );

			// get the inner exception if there is one
			if ( exp.InnerException != null )
			{
				msg.AppendFormat( "{0}; {1} ", msg, exp.InnerException.Message );
				if ( includeStackTrace )
					msg.Append( exp.InnerException.StackTrace );
			}

			return msg.ToString();
		}
	}
}
