using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Raydreams.Common.Data;
using Raydreams.Common.Serializers;

namespace Raydreams.Common.Tests
{
    [TestClass]
    public class MongoDataManagerTests
    {
        private string _connStr = @"";
        private string _db = "MyDB";

        [TestMethod()]
        public void SimpleDBTest()
        {
            TestRepository repo = new TestRepository(_connStr, _db, "Test");
            Guid id = repo.Insert( new MongoThingy() );
            MongoThingy results = repo.Get( id );

            Assert.IsNotNull( results );
        }
    }

    /// <summary></summary>
    public class MongoThingy
    {
        public MongoThingy()
        {
            this.ID = Guid.NewGuid();
            this.Timestamp = DateTimeOffset.UtcNow;
        }

        [BsonId()]
        [BsonElement( "_id" )]
        [BsonGuidRepresentation( GuidRepresentation.Standard )]
        public Guid ID { get; set; }

        [BsonElement( "started" )]
        [BsonSerializer( typeof( NullableDateTimeOffsetSerializer ) )]
        public DateTimeOffset? Timestamp { get; set; }
    }

    /// <summary></summary>
    public class TestRepository : MongoDataManager<MongoThingy, Guid>
    {
        #region [Fields]

        private string _table = "Test";

        #endregion [Fields]

        #region [Constructors]

        /// <summary></summary>
        public TestRepository( string connStr, string db, string table ) : base( connStr, db )
        {
            this.Table = table;
        }

        #endregion [Constructors]

        #region [Properties]

        /// <summary>The physical Collection name</summary>
        public string Table
        {
            get { return this._table; }
            protected set { if ( !String.IsNullOrWhiteSpace( value ) ) this._table = value.Trim(); }
        }

        #endregion [Properties]

        #region [Methods]

        public MongoThingy Get( Guid id )
        {
            return base.Get( id, this.Table );
        }

        public Guid Insert(MongoThingy item)
        {
            if ( item == null || item.ID == Guid.Empty )
                return Guid.Empty;

            bool results = base.Insert( item, this.Table );

            return ( results ) ? item.ID : Guid.Empty;
        }

        #endregion [Methods]
    }
}