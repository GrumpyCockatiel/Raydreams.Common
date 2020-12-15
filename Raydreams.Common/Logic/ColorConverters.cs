using System;
using System.Drawing;
using System.Linq;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Logic
{
    /// <summary></summary>
    public static class ColorConverters
    {
        /// <summary>Given a string of CMYK colors values [0,100] converts to array of floats [0,1.0]</summary>
        /// <param name="str">CMYK String in the format 45,23,99,12</param>
        /// <returns></returns>
        public static double[] CMYKStrToDouble( this string str )
        {
            double[] results = { 0.0, 0.0, 0.0, 0.0 };

            // split the values on ,
            string[] parts = str.Split( ',', StringSplitOptions.RemoveEmptyEntries );

            for ( int i = 0; i < results.Length; ++i )
            {
                results[i] = Double.Parse( parts[i].Trim() ) / 100.0;
            }

            return results;
        }

        /// <summary>Convert CMYK double array where colors are [0,1.0] to RGB int [0,255]</summary>
        /// <param name="cmyk"></param>
        /// <returns></returns>
        public static int[] CMYKToRGB(double[] cmyk)
		{
            //int[] rgb = new int[] { 0, 0, 0 };

            //rgb[0] = Convert.ToInt32( Math.Round( 255.0 * ( 1.0 - cmyk[0] ) * ( 1.0 - cmyk[3] ) ) );
            //rgb[1] = Convert.ToInt32( Math.Round( 255.0 * ( 1.0 - cmyk[1] ) * ( 1.0 - cmyk[3] ) ) );
            //rgb[2] = Convert.ToInt32( Math.Round( 255.0 * ( 1.0 - cmyk[2] ) * ( 1.0 - cmyk[3] ) ) );

            double c = ( cmyk[0] * ( 1.0 - cmyk[3] ) + cmyk[3] );
            double m = ( cmyk[1] * ( 1.0 - cmyk[3] ) + cmyk[3] );
            double y = ( cmyk[2] * ( 1.0 - cmyk[3] ) + cmyk[3] );

            int r = Convert.ToInt32( Math.Round( ( 1 - c ) * 255.0 ) );
            int g = Convert.ToInt32( Math.Round( ( 1 - m ) * 255.0 ) );
            int b = Convert.ToInt32( Math.Round( ( 1 - y ) * 255.0 ) );

            return new int[] { r, g, b };
        }

        /// <summary>Write a byte array as a hex string</summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
		public static string RGBIntsToHex( byte[] rgb )
		{
			return rgb.ToHexString().ToUpper();
		}

        /// <summary></summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static Color CMYKStringToColor( string cmykStr )
        {
            //string[] parts = cmyk.Split( ',', StringSplitOptions.RemoveEmptyEntries );

            //int[] rgb = CMYKToRGB( new int[] { Int32.Parse( parts[0] ), Int32.Parse( parts[1] ), Int32.Parse( parts[2] ), Int32.Parse( parts[3] ) } );

            double[] cmyk = CMYKStrToDouble( cmykStr );
            int[] rgb = CMYKToRGB( cmyk );

            return Color.FromArgb( rgb[0] , rgb[1], rgb[2] );
        }

        /// <summary></summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static Color RGBStringToColor( string rgb )
        {
            string[] parts = rgb.Split( ',', StringSplitOptions.RemoveEmptyEntries );

            if ( parts.Length < 1 )
                return Color.Black;

            int[] values = new int[] { 0, 0, 0 };

            Int32.TryParse( parts[0], out values[0] );
            if (parts.Length > 1)
                Int32.TryParse( parts[1], out values[1] );
            if ( parts.Length > 2 )
                Int32.TryParse( parts[2], out values[2] );

            values[0] = values[0].Normalize( 0, 255 );
            values[1] = values[1].Normalize( 0, 255 );
            values[2] = values[2].Normalize( 0, 255 );

            return Color.FromArgb( values[0], values[1], values[2] );
        }

        /// <summary>Convert a hex string to a Color value</summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static Color HexToColor(string hex)
        {
            if ( String.IsNullOrWhiteSpace( hex ) )
                return Color.Black;

            // strip all non hex chars - still need to strip NON hex chars
            hex = new String( hex.Where( c => System.Uri.IsHexDigit( c ) ).ToArray() );

            // pad if less than 6 otherwise just 6
            if ( hex.Length < 6 )
                hex.PadLeft( 6, '0' );
            else
                hex = hex.Substring( 0, 6 );

            return ColorTranslator.FromHtml( $"#{hex}" );
        }

        /// <summary></summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ColorToHexStr( Color c )
        {
            return RGBIntsToHex( new byte[] { c.R, c.G, c.B } );
        }

        /// <summary></summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ColorToRGBStr( Color c )
        {
            return $"{c.R},{c.G},{c.B}";
        }

        /// <summary></summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static string ColorToCMYKStr( Color c )
        {
            double[] cmyk = ColorToCMYK( c );

            double cy = cmyk[0] * 100.0;
            double m = cmyk[1] * 100.0;
            double y = cmyk[2] * 100.0;
            double k = cmyk[3] * 100.0;

            return String.Format( "{0:0.00},{1:0.00},{2:0.00},{3:0.00}", cy, y, m, k);
        }

        /// <summary>Converts RGB Color to double values between 0 and 1.0</summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] ColorToDouble( Color c )
        {
            double[] rgba = new double[] { 0.0, 0.0, 0.0, 0.0 };

            rgba[0] = c.R / 255.0;
            rgba[1] = c.G / 255.0;
            rgba[2] = c.B / 255.0;
            rgba[3] = c.A / 255.0;

            return rgba;
        }

        /// <summary></summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] ColorToCMYK( Color c )
        {
            double[] rgb = new double[] { 0.0, 0.0, 0.0 };
            double[] cmyk = new double[] { 0.0, 0.0, 0.0, 0.0 };

            rgb[0] = c.R / 255.0;
            rgb[1] = c.G / 255.0;
            rgb[2] = c.B / 255.0;

            cmyk[3] = 1.0 - rgb.Max();
            cmyk[0] = ( 1.0 - rgb[0] - cmyk[3] ) / (1.0 - cmyk[3] );
            cmyk[1] = ( 1.0 - rgb[1] - cmyk[3] ) / ( 1.0 - cmyk[3] );
            cmyk[2] = ( 1.0 - rgb[2] - cmyk[3] ) / ( 1.0 - cmyk[3] );

            return cmyk;
        }

        /// <summary></summary>
        /// <param name="c"></param>
        /// <returns></returns>
		public static double[] ColorToXYZ(Color c)
        {
            double[] rgba = ColorToDouble( c );

            double tempR = 0.0;
            double tempG = 0.0;
            double tempB = 0.0;

            if ( rgba[0] > 0.04045 )
            {
                tempR = Math.Pow( ( rgba[0] + 0.055 ) / 1.055, 2.4 );
            }
            else
            { tempR = rgba[0] / 12.92; }

            if ( rgba[1] > 0.04045 )
            { tempG = Math.Pow( ( rgba[1] + 0.055 ) / 1.055, 2.4 ); }
            else
            { tempG = rgba[1] / 12.92; }

            if ( rgba[2] > 0.04045 )
            { tempB = Math.Pow( ( rgba[2] + 0.055 ) / 1.055, 2.4 ); }
            else
            { tempB = rgba[2] / 12.92;  }

            tempR *= 100;
            tempG *= 100;
            tempB *= 100;

            double x = tempR * 0.4124 + tempG * 0.3576 + tempB * 0.1805;
            double y = tempR * 0.2126 + tempG * 0.7152 + tempB * 0.0722;
            double z = tempR * 0.0193 + tempG * 0.1192 + tempB * 0.9505;

            return new double[] { x, y, z };
        }

        /// <summary></summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double[] ColorToLab( Color c )
        {
            double[] xyz = ColorToXYZ( c );

            // these params could potentially be adjusted
            double refX = 95.047;   //Observer= 2°, Illuminant= D65
            double refY = 100.000;
            double refZ = 108.883;

            double varX = xyz[0] / refX;
            double varY = xyz[1] / refY;
            double varZ = xyz[2] / refZ;

            if ( varX > 0.008856 )
            { varX = Math.Pow( varX, 1.0 / 3.0 ); }
            else
            { varX = ( 7.787 * varX ) + ( 16.0 / 116.0 );  }

            if ( varY > 0.008856 )
            { varY = Math.Pow( varY, 1.0 / 3.0 ); }
            else
            { varY = ( 7.787 * varY ) + ( 16.0 / 116.0 );  }

            if ( varZ > 0.008856 )
            { varZ = Math.Pow( varZ, 1.0 / 3.0 ); }
            else
            { varZ = ( 7.787 * varZ ) + ( 16.0 / 116.0 );  }

            double l = ( 116.0 * varY ) - 16.0;
            double a = 500.0 * ( varX - varY );
            double b = 200.0 * ( varY - varZ );

            return new double[] { l, a, b };
        }

        /// <summary>Takes to colors as Lab arrays of type double</summary>
        /// <param name="c1">Lab color array as double array</param>
        /// <param name="c2">Lab color array as double array</param>
        /// <returns>A DeltaE comparison value</returns>
        public static double CalcDeltaE( double[] c1, double[] c2)
        {
		    return Math.Sqrt( Math.Pow(c2[0] - c1[0], 2) + Math.Pow( c2[1] - c1[1], 2) + Math.Pow( c2[2] - c1[2], 2));
	    }

        /// <summary>Returns JUST the luminance of a specified color, the L in Lab</summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public static double CalcLuminance(this Color c)
        {
            double[] xyz = ColorToXYZ( c );

            double y = xyz[1] / 100.0;

            if ( y > 0.008856 )
            { y = Math.Pow( y, 1.0 / 3.0 ); }
            else
            { y = ( 7.787 * y ) + ( 16.0 / 116.0 ); }

            return ( 116.0 * y ) - 16.0;
        }

    }
}