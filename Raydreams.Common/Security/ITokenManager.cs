using System;
using Newtonsoft.Json;

namespace Raydreams.Common.Security
{
    /// <summary>Delegate function to define the signature of a random salt generator</summary>
    /// <param name="min">Min char len</param>
    /// <param name="max">Max char len</param>
    /// <returns></returns>
    public delegate string MakeTokenSalt( int min = 1, int max = 10 );

    /// <summary>Defines how to serializing and deserilaizing tokens.</summary>
    /// <remarks>
    /// Your token manager decides how to serialize the token
    /// For example, you could just concate all the values with a deliminator and BASE64 encode it if you didnt care about security.
    /// </remarks>
    public interface ITokenManager
    {
        /// <summary>Decode a token back to its object</summary>
        /// <param name="payload">The possibly encrypted token string</param>
        /// <returns>The decrypted token</returns>
        public TokenPayload Decode( string payload );

        /// <summary>Encode a token object into a string using some concrete encryption</summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public string Encode( TokenPayload token );
    }

}