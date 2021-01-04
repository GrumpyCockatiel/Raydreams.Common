using System;
using Newtonsoft.Json;

namespace Raydreams.Common.Security
{
    /// <summary>Options that can be set in the token to give more details. Add as needed.</summary>
    [Flags]
    public enum TokenParam
    {
        /// <summary></summary>
        None = 0,
        /// <summary>is this in dev</summary>
        IsDev = 1,
        /// <summary>is this in production</summary>
        IsProd = 2,
        /// <summary>load mock data instead of real data</summary>
        LoadMocks = 4
    }

    /// <summary>Session Token Base object.</summary>
    /// <remarks>JWT sucks. Add fields here as necessary. I dont use roles in the token since they will be retrieved once the token is validated. I get it people think not having to go to the backend to validate a token is better but its less secure in the long run. How you ultimately decide to serialize this token is up to you.</remarks>
    public class TokenPayload
    {
        /// <summary>The ID of the token which usually comes from some DB or other ID generator or new GUID</summary>
        [JsonProperty( "i" )]
        public string ID { get; set; }

        /// <summary>Some random value to prevent someone from guessing the next ID from your ID generator in case its well known or you stupidly used an sequential generator.</summary>
        [JsonProperty( "s" )]
        public string Salt { get; set; }

        /// <summary>Serialized version of the Metadata token parameters.</summary>
        [JsonProperty( "p" )]
        public int Parameters { get; set; }

        /// <summary>The domain this token is valid for</summary>
        /// <example>Usually something like 'example.com'</example>
        /// <remarks>Optional</remarks>
        [JsonProperty( "d" )]
        public string Domain { get; set; }

        /// <summary>Does the token posses certain optional params</summary>
        /// <param name="p">The param to check for</param>
        /// <returns>true if the flag exists on this token</returns>
        public bool HasFlag( TokenParam p )
        {
            return ( (TokenParam)this.Parameters & p ) > 0;
        }

        /// <summary>Is this token fully formed or not</summary>
        /// <returns></returns>
        public bool IsValid()
        {
            if ( String.IsNullOrWhiteSpace( this.ID ) || String.IsNullOrWhiteSpace( this.Salt ) || String.IsNullOrWhiteSpace( this.Domain ) )
                return false;

            return true;
        }

        /// <summary>Test the Salt and Domain for equality</summary>
        /// <param name="salt"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool IsEqual( string salt, string domain )
        {
            if ( String.IsNullOrWhiteSpace( this.Salt ) || String.IsNullOrWhiteSpace( this.Domain ) )
                return false;

            if ( !salt.Equals( this.Salt, StringComparison.InvariantCulture ) )
                return false;

            if ( !domain.Equals( this.Domain, StringComparison.InvariantCultureIgnoreCase ) )
                return false;

            return true;
        }
    }
}
