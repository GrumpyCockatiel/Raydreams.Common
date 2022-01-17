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
                TableName = "Table2"
            };

            int results = dm.Insert( new TestObject { ID = 1, Text = "Test" } );

            Assert.IsTrue( results > 0 );
        }
    }
}
