using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Microsoft.Azure.Cosmos.Table;
using System.Collections.Generic;
using Raydreams.Common.Serializers;
using Raydreams.Common.Data;
using Raydreams.Common.Extensions;

namespace Raydreams.Common.Logging
{
    /// <summary>Encapsulate all log information into a single object</summary>
    /// <remarks>Designed to work with it without Azure Tables</remarks>
    [BsonIgnoreExtraElements]
    public class LogRecord : ITableEntity
    {
        private Guid _id;

        public LogRecord() : this( null, LogLevel.Info )
        {
        }

        public LogRecord( string message, LogLevel level = LogLevel.Info )
        {
            this.Message = message;
            this.Level = level;

            this.ID = Guid.NewGuid();
            this.PartitionKey = "1";
            this.Timestamp = DateTimeOffset.UtcNow;
        }

        /// <summary>Unique ID of the record</summary>
        [BsonId()]
        [BsonElement( "_id" )]
        [BsonGuidRepresentation( GuidRepresentation.Standard )]
        [JsonIgnore]
        [RayProperty( Source = "ID" )]
        public Guid ID
        {
            get => this._id;
            set
            {
                if ( value != Guid.Empty )
                    this._id = value;
            }
        }

        #region [ Azure Table Required Properties ]

        [BsonIgnore]
        [JsonIgnore]
        /// <summary>For now always 1</summary>
        public string PartitionKey { get; set; }

        /// <summary>Use the internal GUID ID as the Row Key in Azure Tables</summary>
        [BsonIgnore]
        [JsonProperty( "id" )]
        public string RowKey
        {
            get { return this._id.ToString(); }
            set
            {
                if ( Guid.TryParse( value, out Guid temp ) )
                    this._id = temp;
            }
        }

        /// <summary>DateTime of the event preferably in UTC</summary>
        [BsonElement( "timestamp" )]
        [BsonSerializer( typeof( DateTimeOffsetSerializer ) )]
        [JsonProperty( PropertyName = "timestamp" )]
        [RayProperty( Source = "Timestamp" )]
        public DateTimeOffset Timestamp { get; set; }

        /// <summary></summary>
        [BsonIgnore]
        [JsonIgnore]
        public string ETag { get; set; }

        #endregion [ Azure Table Required Properties ]

        /// <summary>What was the source of the log - the app, service, ...</summary>
        [BsonElement( "source" )]
        [JsonProperty( "source" )]
        [RayProperty( Source = "Source" )]
        public string Source { get; set; }

        /// <summary>Severity</summary>
        /// <remarks>See enumerated LogLevels in Logging</remarks>
        [BsonElement( "level" )]
        [BsonRepresentation( BsonType.String )]
        [JsonConverter( typeof( StringEnumConverter ) )]
        [JsonProperty( PropertyName = "level" )]
        [RayProperty( Source = "Level" )]
        public LogLevel Level { get; set; }

        /// <summary>An optional category to help orgnaize log events.</summary>
        [BsonElement( "category" )]
        [JsonProperty( PropertyName = "category" )]
        [RayProperty( Source = "Category" )]
        public string Category { get; set; }

        /// <summary>The actual log message</summary>
        [BsonElement( "message" )]
        [JsonProperty( PropertyName = "message" )]
        [RayProperty( Source = "Message" )]
        public string Message { get; set; }

        /// <summary>Additional arguments can be passed as a generic array</summary>
        /// <remarks>Need to create a custom BSON serializer to conver to string[]</remarks>
        [BsonElement( "args" )]
        [BsonSerializer( typeof( ObjToStringArraySerializer ) )]
        [JsonProperty( "args" )]
        public object[] Args { get; set; }

        /// <summary></summary>
        public void ReadEntity( IDictionary<string, EntityProperty> props, OperationContext operationContext )
        {
            this.Source = props["Source"].StringValue;
            this.Message = props["Message"].StringValue;
            this.Level = props["Level"].StringValue.ToEnum<LogLevel>( true );
            this.Category = props["Category"].StringValue;
            string allArgs = props["Args"].StringValue;
            this.Args = allArgs.Split( ';', StringSplitOptions.RemoveEmptyEntries );
        }

        /// <summary></summary>
        public IDictionary<string, EntityProperty> WriteEntity( OperationContext operationContext )
        {
            var props = new Dictionary<string, EntityProperty>
            {
                ["Source"] = new EntityProperty( this.Source ?? String.Empty ),
                ["Message"] = new EntityProperty( this.Message ),
                ["Level"] = new EntityProperty( this.Level.ToString() ),
                ["Category"] = new EntityProperty( this.Category ?? String.Empty ),
                ["Args"] = new EntityProperty( ( this.Args != null && this.Args.Length > 0 ) ? String.Join( ";", this.Args ) : String.Empty )
            };

            return props;
        }
    }
}
