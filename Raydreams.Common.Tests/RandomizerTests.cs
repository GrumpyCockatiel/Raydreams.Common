using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Extensions;
using Raydreams.Common.Logic;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class RandomizerTests
    {
        [TestMethod]
        public void PickPWTest()
        {
            Randomizer rand = new Randomizer();

            string results = rand.RandomCode( 8, CharSet.NoSimilar );

            Assert.IsNotNull( results );
        }
    }
}


