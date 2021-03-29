using System;
using System.Drawing;

namespace Raydreams.Common.Logic
{
    /// <summary></summary>
    public static class Angles
    {
        /// <summary>Precalc Cosine angles as radians floats</summary>
        public static readonly float COS0 = 1.0F;
        public static readonly float COS18 = Convert.ToSingle( Math.Cos( 18.0 * Math.PI / 180.0 ) );
        public static readonly float COS30 = Convert.ToSingle( Math.Cos( 30.0 * Math.PI / 180.0 ) );
        public static readonly float COS45 = Convert.ToSingle( Math.Cos( 45.0 * Math.PI / 180.0 ) );
        public static readonly float COS54 = Convert.ToSingle( Math.Cos( 54.0 * Math.PI / 180.0 ) );
        public static readonly float COS60 = Convert.ToSingle( Math.Cos( 60.0 * Math.PI / 180.0 ) ); // 0.5
        public static readonly float COS90 = Convert.ToSingle( Math.Cos( 90.0 * Math.PI / 180.0 ) );
        public static readonly float COS120 = 0;
        public static readonly float COS126 = Convert.ToSingle( Math.Cos( 126.0 * Math.PI / 180.0 ) );
        public static readonly float COS135 = Convert.ToSingle( Math.Cos( 135.0 * Math.PI / 180.0 ) );
        public static readonly float COS150 = Convert.ToSingle( Math.Cos( 150.0 * Math.PI / 180.0 ) );
        public static readonly float COS162 = Convert.ToSingle( Math.Cos( 162.0 * Math.PI / 180.0 ) );
        public static readonly float COS180 = -1.0F;
        public static readonly float COS198 = Convert.ToSingle( Math.Cos( 198.0 * Math.PI / 180.0 ) );
        public static readonly float COS225 = Convert.ToSingle( Math.Cos( 225.0 * Math.PI / 180.0 ) );
        public static readonly float COS234 = Convert.ToSingle( Math.Cos( 234.0 * Math.PI / 180.0 ) );
        public static readonly float COS240 = Convert.ToSingle( Math.Cos( 240.0 * Math.PI / 180.0 ) );
        public static readonly float COS270 = 0;
        public static readonly float COS300 = Convert.ToSingle( Math.Cos( 300.0 * Math.PI / 180.0 ) );
        public static readonly float COS306 = Convert.ToSingle( Math.Cos( 306.0 * Math.PI / 180.0 ) );
        public static readonly float COS315 = Convert.ToSingle( Math.Cos( 315.0 * Math.PI / 180.0 ) );
        public static readonly float COS342 = Convert.ToSingle( Math.Cos( 342.0 * Math.PI / 180.0 ) );
        public static readonly float COS360 = 1.0F;

        /// <summary>Precalc sin angles as radians</summary>
        public static readonly float SIN0 = 0;
        public static readonly float SIN18 = Convert.ToSingle( Math.Sin( 18.0 * Math.PI / 180.0 ) );
        public static readonly float SIN30 = Convert.ToSingle( Math.Sin( 30.0 * Math.PI / 180.0 ) );
        public static readonly float SIN45 = Convert.ToSingle( Math.Sin( 45.0 * Math.PI / 180.0 ) );
        public static readonly float SIN54 = Convert.ToSingle( Math.Sin( 54.0 * Math.PI / 180.0 ) );
        public static readonly float SIN60 = Convert.ToSingle( Math.Sin( 60.0 * Math.PI / 180.0 ) ); // .8660
        public static readonly float SIN90 = 1.0F
        public static readonly float SIN120 = Convert.ToSingle( Math.Sin( 120.0 * Math.PI / 180.0 ) );
        public static readonly float SIN126 = Convert.ToSingle( Math.Sin( 126.0 * Math.PI / 180.0 ) );
        public static readonly float SIN135 = Convert.ToSingle( Math.Sin( 135.0 * Math.PI / 180.0 ) );
        public static readonly float SIN150 = Convert.ToSingle( Math.Sin( 150.0 * Math.PI / 180.0 ) );
        public static readonly float SIN162 = Convert.ToSingle( Math.Sin( 162.0 * Math.PI / 180.0 ) );
        public static readonly float SIN180 = 0;
        public static readonly float SIN198 = Convert.ToSingle( Math.Sin( 198.0 * Math.PI / 180.0 ) );
        public static readonly float SIN225 = Convert.ToSingle( Math.Sin( 225.0 * Math.PI / 180.0 ) );
        public static readonly float SIN234 = Convert.ToSingle( Math.Sin( 234.0 * Math.PI / 180.0 ) );
        public static readonly float SIN240 = Convert.ToSingle( Math.Sin( 240.0 * Math.PI / 180.0 ) );
        public static readonly float SIN270 = -1.0F;
        public static readonly float SIN300 = Convert.ToSingle( Math.Sin( 300.0 * Math.PI / 180.0 ) );
        public static readonly float SIN306 = Convert.ToSingle( Math.Sin( 306.0 * Math.PI / 180.0 ) );
        public static readonly float SIN315 = Convert.ToSingle( Math.Sin( 315.0 * Math.PI / 180.0 ) );
        public static readonly float SIN342 = Convert.ToSingle( Math.Sin( 342.0 * Math.PI / 180.0 ) );
        public static readonly float SIN360 = 0;

        /// <summary>Returns an angle in Radians from Degrees</summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float Deg2Rad( float deg ) => deg * Convert.ToSingle( Math.PI / 180.0F );

        /// <summary>Returns an angle in degrees from radians</summary>
        public static float Rad2Deg( float rad ) => rad * Convert.ToSingle( 180.0F / Math.PI );

        /// <summary>Returns a point in cartesian coordinates from angle in radians</summary>
        public static PointF Transform(double radians, double radius )
        {
            return new PointF( Convert.ToSingle( radius * Math.Cos(radians) ), Convert.ToSingle( radius * Math.Sin(radians) ) );
        }

        /// <summary>Takes an angle in degrees and list of PointF and translates them</summary>
        /// <param name="angle"></param>
        /// <param name="pts"></param>
        /// <returns></returns>
        public static PointF[] Rotate( float angle, PointF[] pts )
        {
            PointF[] rotPts = new PointF[pts.Length];

            float rad = Deg2Rad( angle );
            float cos = Convert.ToSingle( Math.Cos( rad ) );
            float sin = Convert.ToSingle( Math.Sin( rad ) );

            for ( int i = 0; i < rotPts.Length; ++i )
                rotPts[i] = new PointF( pts[i].X * cos - pts[i].Y * sin, pts[i].X * sin + pts[i].Y * cos );

            return rotPts;
        }

        /// <summary>Takes a set of PointF and moves them by some offset value</summary>
        /// <param name="angle"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static PointF[] Translate( PointF offset, PointF[] pts )
        {
            PointF[] rotPts = new PointF[pts.Length];

            for ( int i = 0; i < rotPts.Length; ++i )
                rotPts[i] = new PointF( pts[i].X + offset.X, pts[i].Y + offset.Y );

            return rotPts;
        }

    }
}
