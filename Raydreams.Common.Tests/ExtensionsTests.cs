using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        /// <summary>Test converting key value pairs in the format key=value,...</summary>
        [TestMethod]
        public void PairsToDictionaryTest()
        {
            string auth = "Digest realm=\"MMS Public API\", domain=\"\", nonce=\"kWVA9Ciu7lNaN5QdjPe8kxPMReVjbt+B\", algorithm=MD5, qop=\"auth\", stale=false";

            Dictionary<string, string> results = auth.PairsToDictionary();

            Assert.IsNotNull( results );
        }

        /// <summary>A place to test regex patterns</summary>
        [TestMethod]
        public void RegexTest()
        {
            //string pattern = "cs:hello()"

            //Regex pattern = new Regex( @"^cs:(\w*)\([\d]\)$", RegexOptions.IgnoreCase );
            //return pattern.IsMatch( str );

            //Dictionary<string, string> results = auth.PairsToDictionary();

            //Assert.IsNotNull( results );
        }
    }
}



