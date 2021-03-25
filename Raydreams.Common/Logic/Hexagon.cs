using System;
using System.Drawing;

namespace Raydreams.Common.Logic
{
    /// <summary>Hexagon shape</summary>
    /// <remarks>Uses PointF from System.Drawing which can be converted to SKPoint easily.</remarks>
	public class Hexagon
    {
        #region [ Fields ]

        /// <summary>The COS of 60 degrees</summary>
        public static float COS60 = Angles.COS60;

        /// <summary>The SIN of 60 degrees</summary>
        public static float SIN60 => Angles.SIN60;

        /// <summary></summary>
        private PointF _origin = new PointF( 0, 0 );

        /// <summary></summary>
        private float _rad = 10.0F;

        /// <summary></summary>
        private PointF[] _points = null;


        #endregion [ Fields ]

        #region [ Constructors ]

        /// <summary>Default of Height = 100</summary>
        public Hexagon( float height = 100.0F ) : this( 0.0F, 0.0F, height )
        {
        }

        /// <summary></summary>
        /// <param name="height">The full height of the hex which can not be less than 2.0F</param>
        public Hexagon( float x, float y, float height )
        {
            if ( height < 2.0F )
                height = 2.0F;

            this._points = new PointF[6];
            this._origin.X = x;
            this._origin.Y = y;

            // this will recalc on the points when called
            this.Height = height;
        }

        #endregion [ Constructors ]

        /// <summary>The distance from the origin to any vertex</summary>
        /// <remarks>The primary way to specify a hex size. Will recalculate.</remarks>
        public float Radius
        {
            get
            {
                return this._rad;
            }
            set
            {
                this._rad = value;
                this.CalcPath();
            }
        }

        /// <summary>Sets or returns the origin</summary>
        /// <remarks>Will recalc if set</remarks>
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
        /// <remarks>Will recalc if set</remarks>
        public float Height
        {
            get { return 2.0F * this.Radius * SIN60; }
            set
            {
                if ( value < 2.0F )
                    value = 2.0F;

                this.Radius = ( value / 2.0F ) / SIN60;
            }
        }

        /// <summary>The length of an edge</summary>
        /// <remarks>Will recalc if set</remarks>
        public float Side
        {
            get { return 2.0F * this.Radius * COS60; }
            set
            {
                this.Radius = ( value / 2.0F ) / COS60;
            }
        }

        /// <summary>the arm is the perpendicular distance from a left or right vertices to the border of the inscribed rect</summary>
        public float Arm => this.Radius - ( this.Side / 2.0F );

        /// <summary>The number of sides which should always be 6 for a hexagon</summary>
        public int NumSides => this._points.Length;

        /// <summary>Get the max width of the hex</summary>
        public float Width => this.Radius * 2.0F;

        /// <summary>The rect that bounds the outside of the hex</summary>
        public RectangleF Bounds => new RectangleF( this.Origin.X - this.Radius,
            this.Origin.Y - ( this.Height / 2.0F ), this.Radius * 2.0F, this.Height );

        /// <summary>Length of a side for an inscribed square</summary>
        public double Inscribed => this.Width * Math.Sqrt( 3.0 ) / ( 1.0 + Math.Sqrt( 3.0 ) );

        /// <summary>The rectangle inscribed inside the hex</summary>
        public RectangleF InscribedRect => new RectangleF( this.Origin.X - ( this.Side / 2.0F ),
            this.Origin.Y - ( this.Height / 2.0F ),
            this.Side, this.Height );

        /// <summary>Gets the vertices</summary>
        public PointF[] Vertices => this._points;

        /// <summary>Calcualtes all the vertices and stores them locally</summary>
        /// <remarks>Only call CalcPath after a origin or size change</remarks>
        protected void CalcPath()
        {
            this._points[0] = new PointF( this._origin.X + this.Radius, this._origin.Y );
            this._points[1] = new PointF( this._origin.X + COS60 * this.Radius, this._origin.Y - SIN60 * this.Radius );
            this._points[2] = new PointF( this._origin.X - COS60 * this.Radius, this._origin.Y - SIN60 * this.Radius );
            this._points[3] = new PointF( this._origin.X - this.Radius, this._origin.Y );
            this._points[4] = new PointF( this._origin.X - COS60 * this.Radius, this._origin.Y + SIN60 * this.Radius );
            this._points[5] = new PointF( this._origin.X + COS60 * this.Radius, this._origin.Y + SIN60 * this.Radius );
        }
    }
}