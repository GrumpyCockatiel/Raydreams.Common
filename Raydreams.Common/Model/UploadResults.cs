using System;
using Newtonsoft.Json;

namespace Raydreams.Common.Model
{
    /// <summary>The response back after a file upload</summary>
    /// <remarks>possibly rename</remarks>
    public class UploadResults
    {
        /// <summary>Original filename which should include the extension</summary>
        [JsonProperty( "filename" )]
        public string Filename { get; set; } = String.Empty;

        /// <summary>The byte length of the uploaded file</summary>
        [JsonProperty( "length" )]
        public int Filelength { get; set; } = 0;

        /// <summary>The byte length of the uploaded file</summary>
        [JsonProperty( "contentType" )]
        public string ContentType { get; set; } = String.Empty;

        /// <summary>The optional container name the file was uploaded to</summary>
        [JsonProperty( "container" )]
        public string Container { get; set; } = String.Empty;
    }
}
