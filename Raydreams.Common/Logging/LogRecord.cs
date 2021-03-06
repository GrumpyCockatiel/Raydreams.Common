﻿using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Raydreams.Common.Data;
using Raydreams.Common.Logging;

namespace Raydreams.Common.Logging
{
    /// <summary>Object representing a log event record.</summary>
    [BsonIgnoreExtraElements]
    public class LogRecord
    {
        public LogRecord(LogLevel level)
        {
            this.Level = level;
        }

        public LogRecord() : this(LogLevel.Info)
        {
        }

        /// <summary>Unique ID of the record</summary>
		[RayProperty( Source = "ID")]
        public long ID { get; set; }

        /// <summary>DateTime of the event preferably in UTC</summary>
        [BsonElement("timestamp")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [JsonProperty(PropertyName = "timestamp")]
        [RayProperty( Source = "Timestamp" )]
        public DateTime Timestamp { get; set; }

        /// <summary>What was the source of the log - the app, service, ...</summary>
        [BsonElement("source")]
        [JsonProperty(PropertyName = "source")]
        [RayProperty( Source = "Source" )]
        public string Source { get; set; }

        /// <summary>Severity</summary>
        /// <remarks>See enumerated LogLevels in Logging</remarks>
        [BsonElement("level")]
        [BsonRepresentation(BsonType.String)]
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(PropertyName = "level")]
        [RayProperty( Source = "Level" )]
        public LogLevel Level { get; set; }

        /// <summary>An optional category to help orgnaize log events.</summary>
        [BsonElement("category")]
        [JsonProperty(PropertyName = "category")]
        [RayProperty( Source = "Category")]
        public string Category { get; set; }

        /// <summary>The actual log message</summary>
        [BsonElement("message")]
        [JsonProperty(PropertyName = "message")]
        [RayProperty( Source = "Message")]
        public string Message { get; set; }

        /// <summary>The actual log message</summary>
        public object[] Args { get; set; }
    }
}
