using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace Raydreams.Common.IO
{
	public class Resourcer
	{
		public Resourcer()
		{
		}

		public static string[] LoremIpsum()
        {
			string file = LoadResource( "Raydreams.Common.Resources.lorem.txt" );
        }

		public static async Task<string> LoadResource( string resourceID )
        {
			if ( String.IsNullOrWhiteSpace( resourceID ) )
				return String.Empty;

			var assembly = Assembly.GetExecutingAssembly();

			using Stream stream = assembly.GetManifestResourceStream( resourceID.Trim() );

			if ( stream == null )
				return String.Empty;

			using StreamReader reader = new StreamReader( stream );

			return await reader.ReadToEndAsync();
		}
	}
}

