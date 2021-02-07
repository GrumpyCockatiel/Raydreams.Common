using System;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Logic
{
    /// <summary>Static methods for generating IDs in various ways</summary>
    public static class IDGenerator
    {
        /// <summary>Generates a YouTube style 8 byte ID URL encoded</summary>
        /// <returns>Returns the encoded ID and actual ulong value</returns>
        public static (string, ulong) GenerateYouTubeID( Randomizer rnd )
        {
            // pick 8 random values for our byte array equal to a Unsigned Big Int
            byte[] idbytes = rnd.RandomBytes( 8 );

            return (StringExtensions.BASE64UrlEncode( idbytes ), BitConverter.ToUInt64( idbytes, 0 ));
        }
    }
}
