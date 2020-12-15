using System;

namespace Raydreams.Common.Logic
{
    /// <summary></summary>
    public static class Angles
    {
        /// <summary>Returns an angle in radians from degrees</summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static double Deg2Rad(double angle)
        {
            return System.Math.PI * angle / 180.0;
        }

        /// <summary>Returns an angle in degrees from radians</summary>
        public static double Rad2Deg(double angle)
        {
            return angle * (180.0 / System.Math.PI);
        }

        /// <summary>Returns a point in cartesian coordinates from angle in radians</summary>
        public static Point Transform(double radians, double radius )
        {
            return new Point( radius * Math.Cos(radians), radius * Math.Sin(radians) );
        }
    }
}
