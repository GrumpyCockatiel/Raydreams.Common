using System;
using System.Collections.Generic;
using System.IO;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Raydreams.Common.Logic;
using Raydreams.Common.Model;

namespace Raydreams.Common.Data
{
    /// <summary>Data manager with Azure Files and Blobs</summary>
    /// <remarks>Blob and Container names are CASE SENSITIVE</remarks>
    public class AzureBlobRepository
    {
        #region [ Constructor ]

        /// <summary></summary>
        /// <param name="connStr"></param>
        public AzureBlobRepository( string connStr )
        {
            this.ConnectionString = connStr;
        }

        #endregion [ Constructor ]

        #region [ Properties ]

        /// <summary></summary>
        public string ConnectionString { get; set; } = String.Empty;

        /// <summary>The file to upload as a binary stream</summary>
        public Stream ImageFile { get; set; }

        /// <summary>MIME Type</summary>
        public string ContentType { get; set; }

        /// <summary></summary>
        /// <remarks>May no longer need this property</remarks>
        public string FileName { get; set; }

        #endregion [ Properties ]

        #region [ Methods ]

        /// <summary>Reads a file into a JSON wrapper conveted to BASE64 from an actual physical file source</summary>
        /// <param name="filePath">Local file path</param>
        /// <returns></returns>
        public static BinaryFileWrapper ReadFile( string filePath )
        {
            BinaryFileWrapper results = new BinaryFileWrapper();

            FileInfo info = new FileInfo( filePath );

            if ( !info.Exists )
                return results;

            results.Filename = info.Name;
            results.ContentType = MimeTypeMap.GetMimeType( info.Extension );

            byte[] data = File.ReadAllBytes( filePath );
            results.Data = Convert.ToBase64String( data, Base64FormattingOptions.None );

            return results;
        }

        /// <summary>Check to see if a blob already exists in the specified container</summary>
        /// <param name="containerName"></param>
        /// <param name="blobName">File or blob name to check for</param>
        /// <returns></returns>
        /// <remarks>Remember to include a file extension</remarks>
        public bool BlobExists( string containerName, string blobName )
        {
            // blob container name - can we set a default somehow
            if ( containerName == null || blobName == null )
                throw new System.ArgumentNullException( "Arguments can not be null." );

            containerName = containerName.Trim();
            blobName = blobName.Trim();

            if ( containerName == String.Empty || blobName == String.Empty )
                return false;

            // Get a reference to a share and then create it
            BlobContainerClient container = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = container.Exists();
            if ( !exists.Value )
                return false;

            // Get a reference to the blob name
            BlobClient blob = container.GetBlobClient( blobName );
            exists = blob.Exists();

            return exists.Value;
        }

        /// <summary></summary>
        /// <param name="containerName"></param>
        /// <param name="blobName"></param>
        /// <returns></returns>
        public BinaryFileWrapper GetBlob( string containerName, string blobName )
        {
            BinaryFileWrapper results = new BinaryFileWrapper();

            // validate input
            if ( String.IsNullOrWhiteSpace( containerName ) || String.IsNullOrWhiteSpace( blobName ) )
                return results;

            containerName = containerName.Trim();
            blobName = blobName.Trim();

            // Get a reference to a share and then create it
            BlobContainerClient container = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = container.Exists();
            if ( !exists.Value )
                return results;

            // set options
            BlobOpenReadOptions op = new BlobOpenReadOptions( false );

            // read the blob to an array
            BlobClient blob = container.GetBlobClient( blobName );
            using Stream stream = blob.OpenRead( op );
            byte[] data = new byte[stream.Length];
            stream.Read( data, 0, data.Length );
            stream.Close();

            // convert to BASE64
            results.Data = Convert.ToBase64String( data, Base64FormattingOptions.None );

            // get the properties
            BlobProperties props = blob.GetProperties().Value;

            if ( props == null )
                return results;
            
            results.ContentType = props.ContentType;
            if ( props.Metadata.ContainsKey("filename") )
                results.Filename = props.Metadata["filename"].ToString();
            
            return results;
        }

        /// <summary>Get a list of all blobs in the specified contaier</summary>
        /// <param name="containerName"></param>
        /// <returns></returns>
        /// <remarks>Still need to determine what we need back for each blob</remarks>
        public List<string> ListBlobs( string containerName )
        {
            List<string> blobs = new List<string>();

            // blob container name - can we set a default somehow
            if ( String.IsNullOrWhiteSpace( containerName ) )
                return new List<string>();

            // Get a reference to a share and then create it
            BlobContainerClient container = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = container.Exists();
            if ( !exists.Value )
                return new List<string>();

            Pageable<BlobItem> results = container.GetBlobs();

            IEnumerator<BlobItem> enu = results.GetEnumerator();
            while ( enu.MoveNext() )
            {
                blobs.Add( enu.Current.Name );
            };

            return blobs;
        }

        /// <summary>Loads a physical file from a local path</summary>
        /// <param name="filePath"></param>
        public void LoadImageFile( string filePath )
        {
            this.ImageFile = new MemoryStream();

            var info = new FileInfo( filePath );

            if ( !info.Exists )
                return;

            // need the orignal name and extension
            //var attr = File.GetAttributes( filePath );
            this.ContentType = MimeTypeMap.GetMimeType( info.Extension );
            this.FileName = info.Name;

            // read in the raw byets from a file
            using ( FileStream file = new FileStream( filePath, FileMode.Open, FileAccess.Read ) )
            {
                file.CopyTo( this.ImageFile );
            }

            // reset the pointer
            this.ImageFile.Position = 0;
        }

        /// <summary>Uploads a blob from a FileWrapper instance</summary>
        /// <param name="file"></param>
        /// <param name="containerName">Container to load to</param>
        /// <param name="blobName">Optional blob name. Random will be assigned if null</param>
        /// <returns></returns>
        public string UploadBlob( BinaryFileWrapper file, string containerName, string blobName = null )
        {
            if ( file == null || !file.IsValid )
                return null;

            byte[] fileData = Convert.FromBase64String( file.Data );
            this.FileName = file.Filename;
            this.ContentType = file.ContentType;

            this.ImageFile = new MemoryStream( fileData );

            return this.UploadBlob( containerName, blobName );
        }

        /// <summary>Upload a blob to azure storage. Assumes all properties are set mnaually before upload.</summary>
        /// <param name="containerName">The name of the blob contanier to upload to</param>
        public string UploadBlob( string containerName, string blobName = null )
        {
            // blob container name - can we set a default somehow
            if ( String.IsNullOrWhiteSpace( containerName ) )
                //containerName = "/";
                return null;

            // validate
            if ( this.ImageFile == null || this.ImageFile.Length < 1 )
                return null;

            if ( String.IsNullOrWhiteSpace( this.ContentType ) )
                this.ContentType = MimeTypeMap.DefaultMIMEType;

            // reset the pointer
            this.ImageFile.Position = 0;

            // pick a new random name if non was supplied
            if ( String.IsNullOrWhiteSpace( blobName ) )
            {
                Randomizer rnd = new Randomizer();
                blobName = $"{rnd.RandomCode( 11 )}{MimeTypeMap.GetExtension( this.ContentType )}";
            }
            else
                blobName = blobName.Trim();

            // Get a reference to a share and then create it
            BlobContainerClient container = new BlobContainerClient( this.ConnectionString, containerName );

            // check the container exists
            Response<bool> exists = container.Exists();
            if ( !exists.Value )
                return null;
            //container.Create();

            // Get a reference to the blob name
            BlobClient blob = container.GetBlobClient( blobName );
            // what if the name already exist in the container

            // Upload local file
            Dictionary<string, string> meta = new Dictionary<string, string>()
            {
                { "filename", this.FileName }
                // optional description
            };

            // set options
            BlobUploadOptions op = new BlobUploadOptions { HttpHeaders = new BlobHttpHeaders { ContentType = this.ContentType }, Metadata = meta };

            // does this sig already exist?

            // upload - returns the eTag and MD5 file sig
            Response<BlobContentInfo> resp = blob.Upload( this.ImageFile, op );

            // clear the memory stream
            this.ImageFile.Dispose();

            // return the used file name
            return blobName;
        }

        #endregion [ Methods ]
    }
}
