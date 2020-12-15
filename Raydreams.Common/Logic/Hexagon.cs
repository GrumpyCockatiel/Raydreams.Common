using System;
using System.Drawing;

namespace Raydreams.Common.Logic
{
    /// <summary></summary>
	public class Hexagon
	{
        public static float COS60 = Convert.ToSingle(Math.Cos(60.0 * Math.PI / 180.0)); // .5
        public static float SIN60 = Convert.ToSingle(Math.Sin(60.0 * Math.PI / 180.0)); // .8660

        private PointF _origin = new PointF(0, 0);
        private float _side = 0.0F;
        private PointF[] _points = null;
        private int _id = 0;

        public Hexagon()
            : this(0.0F, 0.0F, 100.0F)
        {
        }

        public Hexagon(float side) : this(0.0F, 0.0F, side)
        {
        }

        public Hexagon(float x, float y, float side)
        {
            this._points = new PointF[6];
            this._origin.X = x;
            this._origin.Y = y;
            this.Side = side;
        }

        public PointF Origin
        {
            get { return this._origin; }
            set
            {
                this._origin = value;
                this.CalcPath();
            }
        }

        /// <summary>The height of the hex</summary>
        public float Height
        {
            get { return this.Radius * SIN60; }
        }

        /// <summary>The length of an edge</summary>
        public float Side
        {
            get { return this._side; }
            set
            {
                this._side = value;
                this.CalcPath();
            }
        }

        public int NumSides
        {
            get { return 6; }
        }

        public int ID
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <summary>
        /// get the max width of the hex
        /// </summary>
        public float Width
        {
            get { return this.Radius * 2.0F; }
        }

        /// <summary>length of a side for an inscribed square</summary>
        public double Inscribed
        {
            //get { return 0; }
            get { return this.Width * Math.Sqrt(3.0) / (1.0 + Math.Sqrt(3.0)); }
        }

        public PointF[] Path
        {
            get { return this._points; }
        }

        /// <summary>get the distance from the origin to any vertex</summary>
        public float Radius
        {
            get { return (this._side / 2.0F) / COS60; }
        }

        public void CalcPath()
        {
            this._points[0] = new PointF(this._origin.X + this.Radius, this._origin.Y);
            this._points[1] = new PointF(this._origin.X + COS60 * this.Radius, this._origin.Y - SIN60 * this.Radius);
            this._points[2] = new PointF(this._origin.X - COS60 * this.Radius, this._origin.Y - SIN60 * this.Radius);
            this._points[3] = new PointF(this._origin.X - this.Radius, this._origin.Y);
            this._points[4] = new PointF(this._origin.X - COS60 * this.Radius, this._origin.Y + SIN60 * this.Radius);
            this._points[5] = new PointF(this._origin.X + COS60 * this.Radius, this._origin.Y + SIN60 * this.Radius);
        }
    }
}
