using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void PairsToDictionaryTest()
        {
            string auth = "Digest realm=\"MMS Public API\", domain=\"\", nonce=\"kWVA9Ciu7lNaN5QdjPe8kxPMReVjbt+B\", algorithm=MD5, qop=\"auth\", stale=false";

            Dictionary<string, string> results = auth.PairsToDictionary();

            Assert.IsNotNull( results );
        }
    }
}
