using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class SQLiteDataManagerTests
    {
        /// <summary>Reads all the items from the SQLite DB file</summary>
        /// <returns></returns>
        [TestMethod]
        public void ReadFromSQLite()
        {
            TestObjectRepo dm = new TestObjectRepo( @"Data Source=/Users/tag/Desktop/test.db" )
            {
                TableName = "Table1"
            };

            List<TestObject> items = dm.SelectAll();

            Assert.IsNotNull(items);
        }

        /// <summary>Reads all the items from the SQLite DB file</summary>
        /// <returns></returns>
        [TestMethod]
        public void WriteToSQLite()
        {
            TestObjectRepo dm = new TestObjectRepo( @"Data Source=/Users/tag/Desktop/test.sqlite" )
            {
                TableName = "Table1"
            };

            int results = dm.Insert( new TestObject { ID = 4, Text = "Test some data", Timestamp = DateTimeOffset.UtcNow, Registered = true, Data = new byte[] { 0x45, 0x45, 0x45, 0x56 } } );

            Assert.IsTrue( results > 0 );
        }
    }
}
