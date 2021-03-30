using System;
using System.ComponentModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Raydreams.Common.IO;
using Raydreams.Common.Model;

namespace Raydreams.Common.Tests
{
    public class TestClass
    {
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( "type" )]
        [DefaultValue( EnvironmentType .Unknown)]
        public EnvironmentType Type { get; set; }
    }

    [TestClass]
    public class CSVTester
    {
        private string quotedStrTest = "\" \",\"TEXPW00001073\",\" \",\"10100055abcd\",\"Active\",\"Leslie\",\"Copper\",\"Leslie.Copper@hrcorp.com\",\"Product Manager, North America\",\" \",\"00609300_Texaco\",\"00609300\",\"10005206\",\"Strategic Sourcing & Category Mgmt - Indirects\",\"T589\",\"TARFIN PW\",\" \",,\"Procurement \",\"AA40011290\",\" \",\"03/23/2015\",,\" \",\"60000951\",\"TX-Houston-1601 Austin Street\",\"92000025\",\"Hr\",\"No\",\"lcopper\",\"No\",\"Yes\",\"No\",\"No\",\"Yes\"";
        private string unquotedStrTest = "AX40JS00000003,OXYWK00000003,z12082320345436806102a71,01055940heja,Active,Jamie,Herring,jherring@company.com, Contract Supplier - Hourly, OXYWO00000003,00031201_Oxy,00031201,10005732, OAWA - Proj Blue Procurement, AX40, Administrative Exchange Inc., Chevron,, GOM Projects Supply Chain, AP40012430,0.00,01/01/2016,12/31/2016, USD,60000951, TX-Houston-1601 Austin Street,00609136, Hr, Yes, jherring, No, No, No, No,";

        /// <summary>Test the CSV line parser with various examples</summary>
        /// <remarks>Use quoted and non-quoted fields as well as mixed, dangling commas, white space strings, quotes within quotes</remarks>
        [TestMethod()]
        public void SimpleParseTest()
        {
            string[] results = ParserUtil.CSVLineReader( unquotedStrTest );

            Assert.IsTrue(results.Length > 0);
        }

        [TestMethod]
        public void JsonSerializeTest()
        {
            string json = "{\"type\":2}";

            TestClass results = JsonConvert.DeserializeObject<TestClass>( json );

            Assert.IsNotNull( results );
        }
    }
}
