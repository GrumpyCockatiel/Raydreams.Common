using System;

namespace Raydreams.Common.Logic
{
    /// <summary></summary>
    public class MinMaxPair
    {
        private double _min = 0.0;
        private double _max = 1.0;

        public MinMaxPair(double min, double max)
        {
            this._min = (min >= max) ? max - 1.0 : min;
            this._max = max;
        }

        public double Min
        { get { return this._min; } }

        public double Max
        { get { return this._max; } }
    }

    /// <summary></summary>
    public class Starfield
    {
        private Point _position = new Point(0, 0);
        private double _rad = 3.0;
        private double _dis = 256.0;

        public Starfield()
        {
            MinMaxPair r = new MinMaxPair(5, 25);
            MinMaxPair z = new MinMaxPair(1, 2);
            this.InitLocation(r, z);
        }

        public Point CurrentPosition
        {
            get { return this._position; }
            set { this._position = value; }
        }

        public double Radius
        {
            get { return this._rad; }
            set { this._rad = value; }
        }

        /// <summary></summary>
        /// <remarks>Z=0 is 256 units away from the screen. When x > 256, the star has moved beyond the screen</remarks>
        public double Distance
        {
            get { return this._dis; }
            set { this._dis = value; }
        }

        /// <summary</summary>
        /// <param name="current">Current location.</param>
        /// <param name="fov">Focal length of the lens</param>
        /// <param name="distance">Z distance of the object. 0 means at the focal length away</param>
        /// <returns></returns>
        public static Point Move(Point current, double fov, double distance)
        {
            Point updated = new Point(0.0, 0.0);
            updated.X = (fov * current.X / distance);
            updated.Y = (fov * current.Y / distance);
            return updated;
        }

        /// <summary></summary>
        private void InitLocation(MinMaxPair r, MinMaxPair z)
        {
            Randomizer rand = new Randomizer();

            double radius = r.Min + (r.Max - r.Min) * rand.Generator.NextDouble();
            double polar = rand.RandomAngle();
            this._position = new Point(radius * Math.Cos(polar), radius * Math.Sin(polar));
            this._dis = z.Min + (z.Max - z.Min) * rand.Generator.NextDouble();
        }

    }
}
