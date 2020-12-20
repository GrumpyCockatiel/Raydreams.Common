using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Extensions;
using Raydreams.Common.Logic;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class AstroAlgoTests
    {
        [TestMethod]
        public void JulianDayTest()
        {
            double jd = DateTime.Now.Date2Julian();

            Assert.IsTrue(jd > 0);
        }

        /// <summary>A test for Sun rise/set</summary>
        [TestMethod()]
        public void RiseSetTranTest()
        {
            int month = 1;          // month
            double day = 1.5;           // day
            int year = 2020;        // year
            double L = 95.0950;     // longitude
            double phi = 29.5070;       // latitude
            double JD;                  // Julian Day
            double[] alpha = new double[3];             // right ascension
            double[] delta = new double[3];             // declination

            TimeSpan ts = TimeSpan.FromDays( day );

            JD = new DateTime( year, month, ts.Days, ts.Hours, ts.Minutes, ts.Seconds ).Date2Julian();

            (double Alpha, double Delta) jd0 = AstroAlgo.ApparentSolarCoordinates( JD );
            (double Alpha, double Delta) jdm1 = AstroAlgo.ApparentSolarCoordinates( JD - 1 );
            (double Alpha, double Delta) jd1 = AstroAlgo.ApparentSolarCoordinates( JD + 1 );

            double[] m = AstroAlgo.RiseTranSet( L, phi, -0.8333, JD,
                new double[] { jdm1.Alpha, jd0.Alpha, jd1.Alpha }, new double[] { jdm1.Delta, jd0.Delta, jd1.Delta } );

            TimeSpan tsRise = AstroAlgo.Fraction2Time( m[1] );
            TimeSpan tsSet = AstroAlgo.Fraction2Time( m[2] );
            //AstroAlgo.AzimuthAltitude(JD + m[1], alpha, delta, L, phi, out A, out h);

            int x = 5;
            Assert.IsTrue( m[1] > 0 );
        }
    }
}
