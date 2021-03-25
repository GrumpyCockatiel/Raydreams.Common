using System;
using System.Drawing;

namespace Raydreams.Common.Logic
{
    /// <summary>Creates primitive shapes in System.Drawing in which the size is specified in a radius of a circle in which they are trascribed.</summary>
    public class Shapes
	{
        /// <summary>Creates a square inscribed inside the spefified circle of radius</summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static PointF[] Square( float radius )
		{
            PointF[] points = new PointF[4];
			points[0] = new PointF( radius * Angles.COS45, radius * Angles.SIN45 );
			points[1] = new PointF( radius * Angles.COS135, radius * Angles.SIN135 );
			points[2] = new PointF( radius * Angles.COS225, radius * Angles.SIN225 );
			points[3] = new PointF( radius * Angles.COS315, radius * Angles.SIN315 );

			return points;
		}

		/// <summary>Creates a diamond inscribed in radius</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
        /// <remarks>Just a rotated square</remarks>
		public static PointF[] Diamond( float radius )
		{
            PointF[] points = new PointF[4];

			points[0] = new PointF( 0, radius );
			points[1] = new PointF( radius, 0 );
			points[2] = new PointF( 0, -radius );
			points[3] = new PointF( -radius, 0 );

			return points;
		}

		/// <summary>Draws a triangle</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF[] Triangle( float radius )
		{
            PointF[] points = new PointF[3];

			points[0] = new PointF( radius * Angles.COS270, radius * Angles.SIN270);
			points[1] = new PointF( radius * Angles.COS30, radius * Angles.SIN30 );
			points[2] = new PointF( radius * Angles.COS150, radius * Angles.SIN150);

			return points;
		}

		/// <summary>Generate points for a star</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF[] Star( float radius )
		{
			return Star( radius, radius / 2.5F );
		}

		/// <summary>Draws a hexagon</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF[] Hexagon( float radius )
		{
			PointF[] points = new PointF[6];

			points[0] = new PointF( radius * Angles.COS0, Angles.SIN0 );
			points[1] = new PointF( radius * Angles.COS60, radius * Angles.SIN60 );
			points[2] = new PointF( radius * Angles.COS120, radius * Angles.SIN120 );
			points[3] = new PointF( radius * Angles.COS180, radius * Angles.SIN180 );
			points[4] = new PointF( radius * Angles.COS240, radius * Angles.SIN240 );
			points[5] = new PointF( radius * Angles.COS300, Angles.SIN300 );

			return points;
		}

		/// <summary>Generate points for a star with inner and outer radius</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF[] Star( float outerRadius, float innerRadius )
		{
			PointF[] points = new PointF[10];

			points[0] = new PointF( outerRadius * Angles.COS270, outerRadius * Angles.SIN270);
			points[1] = new PointF( innerRadius * Angles.COS306, innerRadius * Angles.SIN306 );
			points[2] = new PointF( outerRadius * Angles.COS342, outerRadius * Angles.SIN342 );
			points[3] = new PointF( innerRadius * Angles.COS18, innerRadius * Angles.SIN18 );
			points[4] = new PointF( outerRadius * Angles.COS54, outerRadius * Angles.SIN54 );
			points[5] = new PointF( innerRadius * Angles.COS90, innerRadius * Angles.SIN90 );
			points[6] = new PointF( outerRadius * Angles.COS126, outerRadius * Angles.SIN126 );
			points[7] = new PointF( innerRadius * Angles.COS162, innerRadius * Angles.SIN162 );
			points[8] = new PointF( outerRadius * Angles.COS198, outerRadius * Angles.SIN198 );
			points[9] = new PointF( innerRadius * Angles.COS234, innerRadius * Angles.SIN234 );

			return points;
		}

		/// <summary>Draws a cross or + symbol</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF[] Cross( float radius )
		{
			return Cross( radius, radius / 2.0F );
		}

		/// <summary>Draws a cross or + symbol</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF[] Cross( float outerRadius, float innerRadius )
		{
			PointF[] points = new PointF[12];

			points[0] = new PointF( innerRadius * Angles.COS45, innerRadius * Angles.SIN45 );
			points[3] = new PointF( innerRadius * Angles.COS315, innerRadius * Angles.SIN315 );
			points[6] = new PointF( innerRadius * Angles.COS225, innerRadius * Angles.SIN225 );
			points[9] = new PointF( innerRadius * Angles.COS135, innerRadius * Angles.SIN135 );


			points[1] = new PointF( outerRadius, points[0].Y );
			points[2] = new PointF( outerRadius, points[3].Y );
			points[4] = new PointF( points[3].X, -outerRadius );
			points[5] = new PointF( points[6].X, -outerRadius );
			points[7] = new PointF( -outerRadius, points[6].Y );
			points[8] = new PointF( -outerRadius, points[9].Y );
			points[10] = new PointF( points[9].X, outerRadius );
			points[11] = new PointF( points[0].X, outerRadius );

			return points;
		}

		/// <summary>Draws an X</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF[] Ex( float radius )
		{
			return Ex( radius, radius / 2.0F );
		}

		/// <summary>Draws and X which is just a rotated +</summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static PointF[] Ex( float outerRadius, float innerRadius )
		{
			return Angles.Rotate( 45.0F, Cross( outerRadius, innerRadius ) );
		}

	}
}
