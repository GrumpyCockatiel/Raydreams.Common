using System;
using Newtonsoft.Json;

namespace Raydreams.Common.Security
{
    /// <summary>Delegate function to define the signature of a random salt method</summary>
    /// <param name="min">Min char len</param>
    /// <param name="max">Max char len</param>
    /// <returns></returns>
    public delegate string MakeTokenSalt( int min = 1, int max = 10 );

    /// <summary>Handles serializing and deserilaizing session tokens</summary>
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