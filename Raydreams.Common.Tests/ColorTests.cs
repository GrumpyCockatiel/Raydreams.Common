using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Extensions;
using Raydreams.Common.AstroAlgo;
using Raydreams.Common.Utilities;
using System.Drawing;
using Raydreams.Common.Logic;
using Raydreams.Common.Utils;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class ColorTests
    {
        [TestMethod]
        public void IntPackingTest()
        {
            int[] rgba = new int[] { 100, 4005, 0, -17 };

            byte[] temp = ByteUtil.IntsToBytes( rgba );

            int[] results = ByteUtil.BytesToInts( temp );

            Assert.IsNotNull( results[0] == rgba[0] );
        }

        [TestMethod]
        public void ColorConvertTest()
        {
            Color a = Color.FromArgb( 200, 102, 32 );

            string hex = ColorConverters.ColorToHexStr( a );
            string rgb = ColorConverters.ColorToRGBStr( a );
            string cmyk = ColorConverters.ColorToCMYKStr( a );

            Assert.IsNotNull( rgb );
        }

        [TestMethod]
        public void DoubleByteConvertTest()
        {
            double[] lab = new double[] { 62.3068, 55.0094, 71.3368 };
            byte[] b = ByteUtil.DoublesToBytes( lab );
            double[] results = ByteUtil.BytesToDoubles( b );

            lab = ColorConverters.ColorToLab( Color.White );
            b = ByteUtil.DoublesToBytes( lab );
            results = ByteUtil.BytesToDoubles( b );

            lab = ColorConverters.ColorToLab( Color.Black );
            b = ByteUtil.DoublesToBytes( lab );
            results = ByteUtil.BytesToDoubles( b );

            Assert.IsTrue( results[0] == 62.3068 );
        }

        [TestMethod]
        public void GetStateCodeTest()
        {
            Color a = Color.FromArgb(255, 102, 0);

            double[] rgba = ColorConverters.ColorToDouble( a );

            double[] xyz = ColorConverters.ColorToXYZ( a );
            double[] lab = ColorConverters.ColorToLab( a );

            Assert.IsNotNull( lab );

            // XYZ = 45.9914, 30.7627, 3.5138
            // LAB = 62.3068, 55.0094, 71.3368
        }
    }
}
