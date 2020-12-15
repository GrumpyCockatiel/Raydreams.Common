using System;

namespace Raydreams.Common.Logic
{
	/// <summary></summary>
	public struct Point
    {
        public Point(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }

        public double X;
        public double Y;
    }

    /// <summary></summary>
    public class Shapes
	{
        /// <summary></summary>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static Point[] Square( double radius )
		{
            Point[] _points = new Point[4];

			_points[0] = new Point( radius * Math.Cos( DegreeToRadian( 45 ) ), radius * Math.Sin( DegreeToRadian( 45 ) ) );
			_points[1] = new Point( radius * Math.Cos( DegreeToRadian( 135 ) ), radius * Math.Sin( DegreeToRadian( 135 ) ) );
			_points[2] = new Point( radius * Math.Cos( DegreeToRadian( 225 ) ), radius * Math.Sin( DegreeToRadian( 225 ) ) );
			_points[3] = new Point( radius * Math.Cos( DegreeToRadian( 315 ) ), radius * Math.Sin( DegreeToRadian( 315 ) ) );

			return _points;
		}
		
		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Diamond( double radius )
		{
            Point[] _points = new Point[4];

			_points[0] = new Point( 0, radius );
			_points[1] = new Point( radius, 0 );
			_points[2] = new Point( 0, -radius );
			_points[3] = new Point( -radius, 0 );

			return _points;
		}

		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Triangle( double radius )
		{
            Point[] _points = new Point[3];

			_points[0] = new Point( radius * Math.Cos( DegreeToRadian( 270 ) ), radius * Math.Sin( DegreeToRadian( 270 ) ) );
			_points[1] = new Point( radius * Math.Cos( DegreeToRadian( 30 ) ), radius * Math.Sin( DegreeToRadian( 30 ) ) );
			_points[2] = new Point( radius * Math.Cos( DegreeToRadian( 150 ) ), radius * Math.Sin( DegreeToRadian( 150 ) ) );

			return _points;
		}

		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Star( double radius )
		{
			return Star( radius, radius / 2.5 );
		}

		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Hexagon( double radius )
		{
			Point[] _points = new Point[6];

			_points[0] = new Point( radius * Math.Cos( DegreeToRadian( 0 ) ), radius * Math.Sin( DegreeToRadian( 0 ) ) );
			_points[1] = new Point( radius * Math.Cos( DegreeToRadian( 60 ) ), radius * Math.Sin( DegreeToRadian( 60 ) ) );
			_points[2] = new Point( radius * Math.Cos( DegreeToRadian( 120 ) ), radius * Math.Sin( DegreeToRadian( 120 ) ) );
			_points[3] = new Point( radius * Math.Cos( DegreeToRadian( 180 ) ), radius * Math.Sin( DegreeToRadian( 180 ) ) );
			_points[4] = new Point( radius * Math.Cos( DegreeToRadian( 240 ) ), radius * Math.Sin( DegreeToRadian( 240 ) ) );
			_points[5] = new Point( radius * Math.Cos( DegreeToRadian( 300 ) ), radius * Math.Sin( DegreeToRadian( 300 ) ) );

			return _points;
		}

		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Star( double outerRadius, double innerRadius )
		{
			Point[] _points = new Point[10];

			_points[0] = new Point( outerRadius * Math.Cos( DegreeToRadian( 270 ) ), outerRadius * Math.Sin( DegreeToRadian( 270 ) ) );
			_points[1] = new Point( innerRadius * Math.Cos( DegreeToRadian( 306 ) ), innerRadius * Math.Sin( DegreeToRadian( 306 ) ) );
			_points[2] = new Point( outerRadius * Math.Cos( DegreeToRadian( 342 ) ), outerRadius * Math.Sin( DegreeToRadian( 342 ) ) );
			_points[3] = new Point( innerRadius * Math.Cos( DegreeToRadian( 18 ) ), innerRadius * Math.Sin( DegreeToRadian( 18 ) ) );
			_points[4] = new Point( outerRadius * Math.Cos( DegreeToRadian( 54 ) ), outerRadius * Math.Sin( DegreeToRadian( 54 ) ) );
			_points[5] = new Point( innerRadius * Math.Cos( DegreeToRadian( 90 ) ), innerRadius * Math.Sin( DegreeToRadian( 90 ) ) );
			_points[6] = new Point( outerRadius * Math.Cos( DegreeToRadian( 126 ) ), outerRadius * Math.Sin( DegreeToRadian( 126 ) ) );
			_points[7] = new Point( innerRadius * Math.Cos( DegreeToRadian( 162 ) ), innerRadius * Math.Sin( DegreeToRadian( 162 ) ) );
			_points[8] = new Point( outerRadius * Math.Cos( DegreeToRadian( 198 ) ), outerRadius * Math.Sin( DegreeToRadian( 198 ) ) );
			_points[9] = new Point( innerRadius * Math.Cos( DegreeToRadian( 234 ) ), innerRadius * Math.Sin( DegreeToRadian( 234 ) ) );

			return _points;
		}

		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Cross( double radius )
		{
			return Cross( radius, radius / 2.0 );
		}

		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Cross( double outerRadius, double innerRadius )
		{
			Point[] _points = new Point[12];

			_points[0] = new Point( innerRadius * Math.Cos( DegreeToRadian( 45 ) ), innerRadius * Math.Sin( DegreeToRadian( 45 ) ) );
			_points[3] = new Point( innerRadius * Math.Cos( DegreeToRadian( 315 ) ), innerRadius * Math.Sin( DegreeToRadian( 315 ) ) );
			_points[6] = new Point( innerRadius * Math.Cos( DegreeToRadian( 225 ) ), innerRadius * Math.Sin( DegreeToRadian( 225 ) ) );
			_points[9] = new Point( innerRadius * Math.Cos( DegreeToRadian( 135 ) ), innerRadius * Math.Sin( DegreeToRadian( 135 ) ) );


			_points[1] = new Point( outerRadius, _points[0].Y );
			_points[2] = new Point( outerRadius, _points[3].Y );
			_points[4] = new Point( _points[3].X, -outerRadius );
			_points[5] = new Point( _points[6].X, -outerRadius );
			_points[7] = new Point( -outerRadius, _points[6].Y );
			_points[8] = new Point( -outerRadius, _points[9].Y );
			_points[10] = new Point( _points[9].X, outerRadius );
			_points[11] = new Point( _points[0].X, outerRadius );

			return _points;
		}

		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Ex( double radius )
		{
			return Ex( radius, radius / 2.0 );
		}

		/// <summary></summary>
		/// <param name="radius"></param>
		/// <returns></returns>
		public static Point[] Ex( double outerRadius, double innerRadius )
		{
			return Rotate( 45.0, Cross( outerRadius, innerRadius ) );
		}

		/// <summary></summary>
		/// <param name="angle">Angle in degrees</param>
		/// <returns></returns>
		public static double DegreeToRadian( double angle )
		{
			return Math.PI * (angle / 180.0);
		}

        /// <param name="angle">Angle in radians</param>
        public static double Rad2Deg(double angle)
        {
            return 180.0 * (angle / Math.PI);
        }

        /// <summary></summary>
        /// <param name="angle"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        public static Point[] Rotate( double angle, Point[] pts)
		{
			Point[] rotPts = new Point[pts.Length];

			double rad = DegreeToRadian(angle);

			for ( int i = 0; i < rotPts.Length; ++i )
			{
				rotPts[i] = new Point( pts[i].X * Math.Cos( rad ) - pts[i].Y * Math.Sin( rad ), 
					pts[i].X * Math.Sin( rad ) + pts[i].Y * Math.Cos( rad ) );
			}

			return rotPts;
		}

		/// <summary></summary>
		/// <param name="angle"></param>
		/// <param name="?"></param>
		/// <returns></returns>
		public static Point[] Translate(Point offset, Point[] pts)
		{
			Point[] rotPts = new Point[pts.Length];

			for (int i = 0; i < rotPts.Length; ++i)
				rotPts[i] = new Point( pts[i].X + offset.X, pts[i].Y + offset.Y );

			return rotPts;
		}

	}
}
