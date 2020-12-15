using System;
using Newtonsoft.Json;

namespace Raydreams.Common.Security
{
    /// <summary>Session Token structure</summary>
    public class TokenPayload
    {
        /// <summary>The Session ID in the DB</summary>
        [JsonProperty( "i" )]
        public string ID { get; set; }

        /// <summary>Some Session hint that allow for quick validation. Check this property immediately after decryption for the correct value.</summary>
        //[JsonProperty( "h" )]
        //public string Hint { get; set; }

        /// <summary>Some random Session Salt to prevent someone from guessing the next Session ID from your DB primary key generator.</summary>
        [JsonProperty( "s" )]
        public string Salt { get; set; }

        /// <summary>serialized version of the Metadata token parameters</summary>
        [JsonProperty( "p" )]
        public int Parameters { get; set; }

        /// <summary>Need to store the domain for this token so the gateway knows how to fire-up</summary>
        [JsonProperty( "d" )]
        public string Domain { get; set; }

        /// <summary>Does the token posses certain optional params</summary>
        /// <param name="p"></param>
        /// <returns></returns>
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

    /// <summary>Options that can be set in the token to give more details</summary>
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
}
