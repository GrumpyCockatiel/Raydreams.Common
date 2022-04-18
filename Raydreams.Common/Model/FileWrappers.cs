using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Raydreams.Common.Model
{
    /// <summary>Wraps a BASE64'd binary file in JSON with some basic metadata</summary>
    /// <remarks>This is used to send back to a client as BASE64.</remarks>
    [BsonIgnoreExtraElements]
    public class JSONFileWrapper
    {
        /// <summary>Original filename which should include the extension</summary>
        [BsonElement( "filename" )]
        [JsonProperty( "filename" )]
        public string Filename { get; set; } = String.Empty;

        /// <summary>The MIME Type of the file.</summary>
        /// <remarks>Need to modify so if the filename is set, the MIME type gets set as well</remarks>
        [BsonElement( "contentType" )]
        [JsonProperty( "contentType" )]
        public string ContentType { get; set; } = MimeTypeMap.DefaultMIMEType;

        /// <summary>The actual file bytes as BASE64 encoding</summary>
        [BsonIgnore]
        [JsonProperty( "data" )]
        public string Data { get; set; } = String.Empty;

        /// <summary>The byte length of the original decoded data</summary>
        [BsonElement( "length" )]
        [JsonProperty( "length" )]
        public long Filelength { get; set; } = 0;

        /// <summary>Quick check the object has everything to be valid</summary>
        /// <remarks>ContentType is optional since it can fallback to checking the filename or assume its a default.</remarks>
        [BsonIgnore]
        [JsonProperty( "isValid" )]
        public bool IsValid
        {
            get { return !String.IsNullOrWhiteSpace( this.Filename ) && this.Data != null && this.Data.Length > 0; }
        }
    }

    /// <summary>Wraps raw bytes with some additional metadata</summary>
    /// <remarks>The data is stored as a byte array</remarks>
    public class RawFileWrapper
    {
        /// <summary>Original filename which should include the extension</summary>
        public string Filename { get; set; } = String.Empty;

        /// <summary>The MIME Type of the file.</summary>
        /// <remarks>Need to modify so if the filename is set, the MIME type gets set as well</remarks>
        public string ContentType { get; set; } = MimeTypeMap.DefaultMIMEType;

        /// <summary>The actual file bytes</summary>
        public byte[] Data { get; set; } = new byte[0];

        /// <summary>Quick check the object has everything to be valid</summary>
        /// <remarks>ContentType is optional since it can fallback to checking the filename or assume its a default.</remarks>
        [JsonProperty( "isValid" )]
        public bool IsValid
        {
            get { return !String.IsNullOrWhiteSpace( this.Filename ) && this.Data != null && this.Data.Length > 0; }
        }
    }

}
