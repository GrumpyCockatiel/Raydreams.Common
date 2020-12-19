using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Data;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class DataTests
    {
        private class Tester
        {
            [RayProperty( Source = "Field1" )]
            public string Field1 { get; set; }
        }

        [TestMethod]
        public void RayPropertyTest()
        {

        }
    }
}
