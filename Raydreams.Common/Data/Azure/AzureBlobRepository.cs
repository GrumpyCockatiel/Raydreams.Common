﻿using System;
using System.Collections.Generic;
using System.IO;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Raydreams.Common.Logic;
using Raydreams.Common.Model;

namespace Raydreams.Common.Data
{
    /// <summary>Data manager with Azure Files and Blobs. For now mainly considers Blobs to be image files</summary>
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

        /// <summary>Azure Storage connection string</summary>
        public string ConnectionString { get; set; } = String.Empty;

        /// <summary>Used when uploading an image to storage</summary>
        public Stream ImageFile { get; set; }

        /// <summary>Used when uploading an image to storage</summary>
        public string ContentType { get; set; }

        /// <summary>Used when uploading an image to storage</summary>
        /// <remarks>May no longer need this property</remarks>
        public string FileName { get; set; }

        #endregion [ Properties ]

        #region [ Methods ]

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

        /// <summary>Gets a blob from Azure Storage as just raw bytes with metadata</summary>
        /// <param name="containerName">container name</param>
        /// <param name="blobName">blob name</param>
        /// <returns>Wrapped raw bytes with some metadata</returns>
        public RawFileWrapper GetRawBlob( string containerName, string blobName )
        {
            RawFileWrapper results = new RawFileWrapper();

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
            results.Data = new byte[stream.Length];
            stream.Read( results.Data, 0, results.Data.Length );
            stream.Close();

            // get the properties
            BlobProperties props = blob.GetProperties().Value;

            if ( props == null )
                return results;

            results.ContentType = props.ContentType;

            // get a filename
            if ( props.Metadata.ContainsKey( "filename" ) )
                results.Filename = props.Metadata["filename"].ToString();
            else
                results.Filename = blob.Name;

            return results;
        }

        /// <summary>Gets a blob from Azure Storage BASE64 encoded and wrapped in JSON</summary>
        /// <param name="containerName">container name</param>
        /// <param name="blobName">blob name</param>
        /// <returns>The encoded blob with metadata</returns>
        public JSONFileWrapper GetWrappedBlob( string containerName, string blobName )
        {
            JSONFileWrapper results = new JSONFileWrapper();

            // get the blob
            RawFileWrapper data = this.GetRawBlob( containerName, blobName );

            // validate
            if ( !data.IsValid )
                return results;

            // convert to BASE64
            results.Data = Convert.ToBase64String( data.Data, Base64FormattingOptions.None );

            // get the properties
            results.ContentType = data.ContentType;
            results.Filename = data.Filename;
            
            return results;
        }

        /// <summary>Get a list of All blobs in the specified contaier</summary>
        /// <param name="containerName">container name</param>
        /// <returns>A list of blob names</returns>
        /// <remarks>Still need to determine what we need back for each blob</remarks>
        public List<string> ListBlobs( string containerName, string pattern = null )
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

            Pageable<BlobItem> results = null;
            if ( String.IsNullOrWhiteSpace( pattern ) )
                results = container.GetBlobs();
            else
                results = container.GetBlobs( prefix : pattern.Trim() );

            IEnumerator<BlobItem> enu = results?.GetEnumerator();
            while ( enu.MoveNext() )
            {
                blobs.Add( enu.Current.Name );
            };

            return blobs;
        }

        /// <summary>Uploads a blob from a JSON FileWrapper instance</summary>
        /// <param name="file">The JSON wrapper with the image bytes in BASE64</param>
        /// <param name="containerName">Container to load to</param>
        /// <param name="blobName">Optional blob name. Random will be assigned if null</param>
        /// <returns></returns>
        public string UploadBlob( JSONFileWrapper file, string containerName, string blobName = null )
        {
            if ( file == null || !file.IsValid )
                return null;

            // set the local values
            byte[] fileData = Convert.FromBase64String( file.Data );
            this.FileName = file.Filename;
            this.ContentType = file.ContentType;
            this.ImageFile = new MemoryStream( fileData );

            return this.UploadBlob( containerName, blobName );
        }

        /// <summary>Upload a blob to azure storage. Assumes all properties are set mnaually before uploading.</summary>
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

        /// <summary>Loads a physical file from a local path into storage</summary>
        /// <param name="filePath"></param>
        /// <remarks>This is a helper function to make it easier to load up files</remarks>
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

        /// <summary>Reads a file into a JSON wrapper conveted to BASE64 from an actual physical file source</summary>
        /// <param name="filePath">Local file path</param>
        /// <returns>Helper function</returns>
        public static JSONFileWrapper ReadFile( string filePath )
        {
            JSONFileWrapper results = new JSONFileWrapper();

            FileInfo info = new FileInfo( filePath );

            if ( !info.Exists )
                return results;

            results.Filename = info.Name;
            results.ContentType = MimeTypeMap.GetMimeType( info.Extension );

            byte[] data = File.ReadAllBytes( filePath );
            results.Data = Convert.ToBase64String( data, Base64FormattingOptions.None );

            return results;
        }

        #endregion [ Methods ]
    }
}
