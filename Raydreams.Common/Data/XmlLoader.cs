using System;
using System.Data;

namespace Raydreams.Common.Utilities
{
	public static class XmlLoader
	{
		/// <summary>Loads an XML file into a Dataset given the specified path</summary>
		/// <param name="dsName">Name to assigned the Dataset </param>
		/// <param name="path"></param>
		/// <returns></returns>
		public static DataSet LoadXMLFile(string path, string dsName = null)
		{
			if ( String.IsNullOrWhiteSpace( path ) )
				return null;

			if ( String.IsNullOrWhiteSpace( dsName ) )
				dsName = Guid.NewGuid().ToString();

			dsName = dsName.Trim();

			DataSet ds = new DataSet( dsName );
			XmlReadMode mode = ds.ReadXml( path, XmlReadMode.InferSchema);
			return ds;
		}
	}
}
