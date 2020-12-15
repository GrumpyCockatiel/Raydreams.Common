using System;
using System.Collections.Generic;

namespace Raydreams.Common.Email
{
	/// <summary></summary>
	public static class Templator
	{
		/// <summary>Replaces tokens in text with those from a dictionary</summary>
        /// <param name="template"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        /// <remarks>Keys are in the format $ALLUPPER</remarks>
		public static string Prepare( string template, Dictionary<string, string> fields )
		{
			if (String.IsNullOrWhiteSpace(template))
				return String.Empty;

			if (fields.Count < 1)
				return template;

			template = template.Trim();

			foreach (KeyValuePair<string, string> kvp in fields)
				template = template.Replace( $"${kvp.Key.ToUpper()}", kvp.Value);

			return template;
		}
	}
}
