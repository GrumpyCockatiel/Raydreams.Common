using System;
using System.Collections.Generic;
using Raydreams.Common.Data;

namespace Raydreams.Common.Tests
{
    /// <summary>Simple Object for testing</summary>
    public class TestObject
    {
        /// <summary>Row ID</summary>
        [RayProperty( Destination = "id" )]
        public int ID { get; set; }

        [RayProperty( Destination = "text", Context = null )]
        public string Text { get; set; }

        [RayProperty( Destination = "ts", Context = null )]
        public DateTimeOffset Timestamp { get; set; }

        [RayProperty( Destination = "registered", Context = null )]
        public bool Registered { get; set; }

        [RayProperty( Destination = "data", Context = null )]
        public byte[] Data { get; set; }
    }

    /// <summary>Test DataManager</summary>
    public class TestObjectRepo : SQLiteDataManager
    {
        #region [Fields]

        private string _tableName = null;

        #endregion [Fields]

        public TestObjectRepo( string conn ) : base( conn )
        { }

        #region [Properties]

        /// <summary>The Name of the physical table in the database</summary>
        public string TableName
        {
            get { return this._tableName; }
            set
            {
                if ( !String.IsNullOrWhiteSpace( value ) )
                    this._tableName = value.Trim().ToLower();
            }
        }

        #endregion [Properties]

        public int Insert(TestObject item)
        {
            return base.Insert<TestObject>( item, this.TableName, null );
        }

        public List<TestObject> SelectAll()
        {
            return this.SelectAll<TestObject>( this.TableName );
        }
    }
}
