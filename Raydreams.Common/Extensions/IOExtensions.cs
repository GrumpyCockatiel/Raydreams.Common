using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.Extensions
{
	/// <summary>Directory Info Extensions</summary>
	public static class DirInfoExtensions
	{
		/// <summary>Return all files in the specified folder path that match the specified filter. Top level only - no children</summary>
		/// <param name="dir"></param>
		/// <param name="filter">Send null to get every file</param>
		/// <returns></returns>
		public static FileInfo[] GetAllFiles( this DirectoryInfo dir, string filter )
		{
			// no filter will match everything
			if ( String.IsNullOrWhiteSpace( filter ) )
				filter = "*";

			if ( dir == null || !dir.Exists )
				return new FileInfo[] { };

			return dir.GetFiles( filter, SearchOption.TopDirectoryOnly );
		}

		/// <summary>Returns the full path to the last created file with the given filter. Only searches in the input path, no children</summary>
		/// <param name="filter">Filter to use on filtering files such as MyFile*</param>
		/// <returns>Full file path</returns>
		public static FileInfo LatestFile( this DirectoryInfo dir, string filter )
		{
			if ( dir == null || !dir.Exists )
				return null;

			if (String.IsNullOrWhiteSpace( filter ) )
				filter = "*";

			return dir.GetFiles( filter ).OrderByDescending( f => f.LastWriteTime ).FirstOrDefault();
		}

		/// <summary>The directory to purge all files from</summary>
		/// <param name="dir">The directory in which to purge files, only mathces the top level</param>
		/// <param name="filter">The filename filter to use in matching. Will match all if none specified.</param>
		/// <param name="createdBefore">Only matches files created before the specifed date. Pass DateTime.Max to match all.</param>
		/// <returns>A int tuple of number of files deleted, total number of files that match the filter in the specified directory</returns>
		public static Tuple<int, int> PurgeDirectory( this DirectoryInfo dir, string filter, DateTime createdBefore )
		{
			int count = 0;

			if ( !dir.Exists )
				return new Tuple<int, int>( count, -1 );

			// get the files
			FileInfo[] files = dir.GetAllFiles( filter ).Where( f => f.CreationTime < createdBefore ).ToArray();

			// delete each file
			foreach ( FileInfo file in files )
			{
				try
				{
					file.Delete();
					++count;
				}
				catch
				{
					// if an error occurs skip and continue
				}
			}

			return new Tuple<int, int>( count, files.Length );
		}
	}
}
