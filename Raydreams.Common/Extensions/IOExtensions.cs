using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Raydreams.Common.Extensions
{
	/// <summary></summary>
	public static class IOExtensions
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
			if (String.IsNullOrWhiteSpace( filter ))
				filter = "*";

			if ( dir == null || !dir.Exists )
				return null;

			return dir.GetFiles( filter ).OrderByDescending( f => f.LastWriteTime ).FirstOrDefault();
		}

		/// <summary>Alternate to get the latest file that uses a string in and out instead of IO objects.</summary>
		/// <param name="path"></param>
		/// <param name="fileFilter"></param>
		/// <returns></returns>
		public static string LatestFile( this string path, string fileFilter )
		{
			DirectoryInfo dir = new DirectoryInfo( path );

			FileInfo locPath = dir.LatestFile( fileFilter );

			return ( locPath == null ) ? null : locPath.FullName;
		}

		/// <summary>Makes a copy of the source file with the same name + the specified suffix in the destination folder</summary>
		/// <param name="suffix">an additional suffix to add to the end of the file name</param>
		public static int CopyFile( this string srcPath, string destPath, string suffix = "", bool overwrite = true )
		{
			if ( String.IsNullOrWhiteSpace( suffix ) )
				suffix = String.Empty;

			FileInfo fi = new FileInfo( srcPath );
			DirectoryInfo di = new DirectoryInfo( destPath );

			if ( !fi.Exists || !di.Exists )
				return 0;

			// Rename the file
			var filePart = Path.GetFileNameWithoutExtension( fi.FullName );
			var filePartExt = Path.GetExtension( fi.FullName );
			var targetPath = Path.Combine( destPath, String.Format( "{0}{1}{2}", filePart, suffix, filePartExt ) );

			try
			{
				File.Copy( fi.FullName, targetPath, overwrite );
			}
			catch ( System.Exception )
			{
				return 0;
			}

			return 1;
		}


		/// <summary>Moves a file from one folder to another the <see cref="SourceFileName"/> to the <see cref="ArchiveFolder"/></summary>
		/// <param name="suffix">an additional suffix to add to the end of the file name</param>
		public static int MoveFile( this string srcPath, string destPath, string suffix = "_bkup" )
		{
			if ( String.IsNullOrWhiteSpace( suffix ) )
				suffix = String.Empty;

			FileInfo fi = new FileInfo( srcPath );
			DirectoryInfo di = new DirectoryInfo( destPath );

			if ( !fi.Exists || !di.Exists )
				return 0;

			// Rename the file
			var filePart = Path.GetFileNameWithoutExtension( fi.FullName );
			var filePartExt = Path.GetExtension( fi.FullName );
			var targetPath = Path.Combine( destPath, String.Format( "{0}{1}{2}", filePart, suffix, filePartExt ) );

			try
			{
				File.Move( fi.FullName, targetPath );
			}
			catch ( System.Exception )
			{
				return 0;
			}

			return 1;
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

		/// <summary>Deletes the directory at the specified path if you have persmission</summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static void DeleteDirectory( this string path )
		{
			if ( !String.IsNullOrWhiteSpace( path ) && Directory.Exists( path ) )
				Directory.Delete( path, true );
		}

		/// <summary>Creates a directory at the specified path if you have permission</summary>
		/// <param name="path"></param>
		/// <returns></returns>
		public static DirectoryInfo CreateDirectory( this string path )
		{
			if ( String.IsNullOrWhiteSpace( path ) )
				return null;

			return System.IO.Directory.CreateDirectory( path );
		}
	}
}
