using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.IO;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class CSVTester
    {
        private string test1 = "\" \",\"TEXPW00001073\",\" \",\"10100055abcd\",\"Active\",\"Leslie\",\"Copper\",\"Leslie.Copper@hrcorp.com\",\"Product Manager, North America\",\" \",\"00609300_Texaco\",\"00609300\",\"10005206\",\"Strategic Sourcing & Category Mgmt - Indirects\",\"T589\",\"TARFIN PW\",\" \",,\"Procurement \",\"AA40011290\",\" \",\"03/23/2015\",,\" \",\"60000951\",\"TX-Houston-1601 Austin Street\",\"92000025\",\"Hr\",\"No\",\"lcopper\",\"No\",\"Yes\",\"No\",\"No\",\"Yes\"";
        private string test2 = "AX40JS00000003,OXYWK00000003,z12082320345436806102a71,01055940heja,Active,Jamie,Herring,jherring@company.com, Contract Supplier - Hourly, OXYWO00000003,00031201_Oxy,00031201,10005732, OAWA - Proj Blue Procurement, AX40, Administrative Exchange Inc., Chevron,, GOM Projects Supply Chain, AP40012430,0.00,01/01/2016,12/31/2016, USD,60000951, TX-Houston-1601 Austin Street,00609136, Hr, Yes, jherring, No, No, No, No,";

        [TestMethod()]
        public void SimpleParseTest()
        {
            string[] results = ParserUtil.CSVLineReader(test2);

            Assert.IsTrue(results.Length > 0);
        }
    }
}
