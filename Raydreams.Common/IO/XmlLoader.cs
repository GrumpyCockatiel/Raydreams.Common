using System;
using System.Data;

namespace Raydreams.Common.IO
{
	/// <summary>XML Utility Class</summary>
	public static class XmlLoader
	{
		/// <summary>Loads an XML file into a Dataset given the specified physical file path</summary>
		/// <param name="dsName">Name to assigned the Dataset </param>
		/// <param name="path"></param>
		/// <returns>A populated DataSet object</returns>
		public static DataSet LoadXMLFile(string path, string name = null)
		{
			// validate
			// also need to check the file exists and it is an XML file
			if ( String.IsNullOrWhiteSpace( path ) )
				return new DataSet();

			// name is optional
			name = ( String.IsNullOrWhiteSpace( name ) ) ? Guid.NewGuid().ToString() : name.Trim();

			// create the data set
			DataSet ds = new DataSet( name );
			_ = ds.ReadXml( path, XmlReadMode.InferSchema);

			// return
			return ds;
		}
	}
}
