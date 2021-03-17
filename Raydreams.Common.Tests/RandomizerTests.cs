using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Extensions;
using Raydreams.Common.Logic;
using Raydreams.Common.Model;

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

        [TestMethod]
        public void SortLoremTest()
        {
            List<string> results = new List<string>();
            List<string> doublew = new List<string>();

            foreach (String s in LoremIpsum.Values)
            {
                if ( !results.Contains( s ) )
                    results.Add( s );
                else
                    doublew.Add( s );
            }

            Assert.IsNotNull( results );
        }
    }
}


