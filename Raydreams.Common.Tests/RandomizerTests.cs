using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Extensions;
using Raydreams.Common.Logic;
using Raydreams.Common.Model;
using static Raydreams.Common.Logic.AstroAlgo;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class RandomizerTests
    {
        [TestMethod]
        public void RandomEnumTest()
        {
            Randomizer rand = new Randomizer();

            ES results = rand.RandomEnum<ES>();
            ES results2 = rand.RandomEnum<ES>();
            ES results3 = rand.RandomEnum<ES>();
            ES results4 = rand.RandomEnum<ES>();
            ES results5 = rand.RandomEnum<ES>();
            ES results6 = rand.RandomEnum<ES>();
            ES results7 = rand.RandomEnum<ES>();

            Assert.IsNotNull( results );
        }

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


