using System;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Raydreams.Common.Extensions;
using Raydreams.Common.Security;

namespace Raydreams.Common.Security
{
    /// <summary>A Null Token manager when you need a simple token manager instance</summary>
    public class NullTokenManager : ITokenManager
    {
        /// <summary>A null token</summary>
        public static TokenPayload NullToken => new TokenPayload
        {
            ID = "1",
            Domain = "null.com",
            Parameters = 0,
            Salt = "null"
        };

        /// <summary>Decode always returns the Null token regardless of the input</summary>
        public TokenPayload Decode( string token )
        {
            return NullToken;
        }

        /// <summary>Returns a legitimate looking token that will work in dev and test</summary>
        /// <param name="payload"></param>
        /// <returns></returns>
        public string Encode( TokenPayload payload = null )
        {
            return "EZR-w_wrEwU$zZwg8JMgPrPNT6d4MqGuoJPsOERRhtvXpi6REJYaBviuuCyZGGfarGe7-kGrcrKW";
        }
    }
}
