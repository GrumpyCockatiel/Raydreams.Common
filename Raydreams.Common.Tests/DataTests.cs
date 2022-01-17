using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.Data;
using Raydreams.Common.IO;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class IOTests
    {
        [TestMethod]
        public void FileReaderTest()
        {
            string path = $"{IOHelpers.DesktopPath}/TestFile.csv";

            DataFileReader<TestObject> reader = new DataFileReader<TestObject>( ParserUtil.CSVLineReader );
            List<TestObject> results = reader.Read(path, null, true);

            Assert.IsNotNull( results );
        }
    }
}
