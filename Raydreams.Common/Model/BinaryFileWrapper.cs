﻿using System;
using Newtonsoft.Json;

namespace Raydreams.Common.Model
{
    /// <summary>Adds metadata to blob data</summary>
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

    /// <summary>Wraps a binary file in JSON with some basic info</summary>
    public class JSONFileWrapper
    {
        /// <summary>Original filename which should include the extension</summary>
        [JsonProperty( "filename" )]
        public string Filename { get; set; } = String.Empty;

        /// <summary>The MIME Type of the file.</summary>
        /// <remarks>Need to modify so if the filename is set, the MIME type gets set as well</remarks>
        [JsonProperty( "contentType" )]
        public string ContentType { get; set; } = MimeTypeMap.DefaultMIMEType;

        /// <summary>The actual file bytes as BASE64 encoding</summary>
        [JsonProperty( "data" )]
        public string Data { get; set; } = String.Empty;

        /// <summary>Quick check the object has everything to be valid</summary>
        /// <remarks>ContentType is optional since it can fallback to checking the filename or assume its a default.</remarks>
        [JsonProperty( "isValid" )]
        public bool IsValid
        {
            get { return !String.IsNullOrWhiteSpace( this.Filename ) && this.Data != null && this.Data.Length > 0; }
        }
    }
}
