using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Raydreams.Common.IO;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class SQLiteDataManagerTests
    {
        string testPath = $"Data Source={IOHelpers.DesktopPath}/test.sqlite";

        /// <summary>Reads all the items from the SQLite DB file</summary>
        /// <returns></returns>
        [TestMethod]
        public void ReadFromSQLiteTest()
        {
            TestObjectRepo dm = new TestObjectRepo( testPath )
            {
                TableName = "Table1"
            };

            List<TestObject> items = dm.SelectAll();

            Assert.IsNotNull(items);
        }

        /// <summary>Test writing a new record to the SQLite DB</summary>
        /// <returns></returns>
        [TestMethod]
        public void TruncateTest()
        {
            TestObjectRepo dm = new TestObjectRepo( testPath )
            {
                TableName = "Table1"
            };

            int results = dm.Truncate();

            Assert.IsTrue( results > 0 );
        }

        /// <summary>Test writing a new record to the SQLite DB</summary>
        /// <returns></returns>
        [TestMethod]
        public void WriteToSQLiteTest()
        {
            TestObjectRepo dm = new TestObjectRepo( testPath )
            {
                TableName = "Table1"
            };

            int results = dm.Insert( new TestObject { ID = 4, Text = "Test some data", Timestamp = DateTimeOffset.UtcNow, Registered = true, Data = new byte[] { 0x45, 0x45, 0x45, 0x56 } } );

            Assert.IsTrue( results > 0 );
        }
    }
}
