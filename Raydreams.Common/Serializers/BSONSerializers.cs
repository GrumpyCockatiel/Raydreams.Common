using System;
using System.Globalization;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Raydreams.Common.Serializers
{
    /// <summary>Converts an class object[] to a BSON string[]</summary>
    public class ObjToStringArraySerializer : SerializerBase<object[]>
    {
        /// <summary>Write the class object[] out as a BSON string[]</summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="value"></param>
        public override void Serialize( BsonSerializationContext context, BsonSerializationArgs args, object[] value )
        {
            // if the incoming array is null
            if ( value == null )
            {
                context.Writer.WriteNull();
                return;
            }

            BsonDocument doc = new BsonDocument();

            for ( int i = 0; i < value.Length; ++i )
            {
                if ( value[i] == null )
                    continue;

                string[] kvp = value[i].ToString().Split( "=", StringSplitOptions.RemoveEmptyEntries );

                if ( kvp.Length > 1 )
                    doc.Add( new BsonElement( kvp[0], kvp[1] ) );
                else if ( kvp.Length > 0 )
                    doc.Add( new BsonElement( $"arg{i + 1}", kvp[0] ) );
            }

            context.Writer.WriteRawBsonDocument( new RawBsonDocument( doc.ToBson() ).Slice );
        }

        /// <summary>Write back to a LogRecord object as an object[]</summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns>The object[] property type of args</returns>
        public override object[] Deserialize( BsonDeserializationContext context, BsonDeserializationArgs args )
        {
            // BsonType should be array
            BsonType bsonType = context.Reader.GetCurrentBsonType();

            if ( bsonType == BsonType.Null )
            {
                context.Reader.ReadNull();
                return null;
            }

            if ( bsonType != BsonType.Document )
                return null;

            IByteBuffer buffer = context.Reader.ReadRawBsonDocument();
            BsonDocument doc = new RawBsonDocument( buffer ).ToBsonDocument();

            if ( doc == null || doc.ElementCount < 1 )
                return null;

            // get all the document values and turn back to object[]
            return doc.ToDictionary().Select( i => $"{i.Key}={i.Value}" ).ToArray();
        }
    }

    /// <summary>Converts a Nullable DateTimeOffset to BSON DateTime</summary>
    public class NullableDateTimeOffsetSerializer : SerializerBase<DateTimeOffset?>
    {
        private string _format = "YYYY-MM-ddTHH:mm:ss.FFFFFFK";

        /// <summary>Deserialize from BSON back to a C# nullable DateTimeOffset</summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override DateTimeOffset? Deserialize( BsonDeserializationContext context, BsonDeserializationArgs args )
        {
            BsonType bsonType = context.Reader.GetCurrentBsonType();

            switch ( bsonType )
            {
                case BsonType.Null:
                    context.Reader.ReadNull();
                    return null;
                case BsonType.String:
                    string value = context.Reader.ReadString();
                    return DateTimeOffset.ParseExact( value, _format, DateTimeFormatInfo.InvariantInfo );

                case BsonType.DateTime:
                    var dateTimeValue = context.Reader.ReadDateTime();
                    return DateTimeOffset.FromUnixTimeMilliseconds( dateTimeValue );

                default:
                    throw CreateCannotDeserializeFromBsonTypeException( bsonType );
            }
        }

        /// <summary>Serialize to BSON from a C# nullable DateTimeOffset</summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <param name="value"></param>
        public override void Serialize( BsonSerializationContext context, BsonSerializationArgs args, DateTimeOffset? value )
        {
            if ( value == null )
            {
                context.Writer.WriteNull();
                return;
            }

            context.Writer.WriteDateTime( ((DateTimeOffset)value).ToUnixTimeMilliseconds() );
        }
    }

    /// <summary>Serialize a C# DateTimeOffset into Mongo timestamp</summary>
    /// <remarks>
    /// from Luke Vosyka
    /// https://www.codeproject.com/Tips/1268086/MongoDB-Csharp-Serializer-for-DateTimeOffset-to-Bs
    /// Got this from some place need to find for credit
    /// </remarks>
    public class DateTimeOffsetSerializer : StructSerializerBase<DateTimeOffset>, IRepresentationConfigurable<DateTimeOffsetSerializer>
    {
        private BsonType _representation;

        private string StringSerializationFormat = "YYYY-MM-ddTHH:mm:ss.FFFFFFK";

        #region [ Constructors ]

        /// <summary>Constructor</summary>
        /// <remarks>Assume as DateTime</remarks>
        public DateTimeOffsetSerializer() : this( BsonType.DateTime )
        {
        }

        public DateTimeOffsetSerializer( BsonType representation )
        {
            switch ( representation )
            {
                case BsonType.String:
                case BsonType.DateTime:
                    break;
                default:
                    throw new ArgumentException( string.Format( "{0} is not a valid representation for {1}", representation, this.GetType().Name ) );
            }

            _representation = representation;
        }

        #endregion [ Constructors ]

        /// <summary></summary>
        public BsonType Representation => _representation;

        /// <summary></summary>
        /// <param name="context"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override DateTimeOffset Deserialize( BsonDeserializationContext context, BsonDeserializationArgs args )
        {
            var bsonReader = context.Reader;

            BsonType bsonType = bsonReader.GetCurrentBsonType();
            switch ( bsonType )
            {
                case BsonType.String:
                    var stringValue = bsonReader.ReadString();
                    return DateTimeOffset.ParseExact
                        ( stringValue, StringSerializationFormat, DateTimeFormatInfo.InvariantInfo );

                case BsonType.DateTime:
                    var dateTimeValue = bsonReader.ReadDateTime();
                    return DateTimeOffset.FromUnixTimeMilliseconds( dateTimeValue );

                default:
                    throw CreateCannotDeserializeFromBsonTypeException( bsonType );
            }
        }

        /// <summary></summary>
        public override void Serialize( BsonSerializationContext context, BsonSerializationArgs args, DateTimeOffset value )
        {
            var bsonWriter = context.Writer;

            switch ( _representation )
            {
                case BsonType.String:
                    bsonWriter.WriteString( value.ToString ( StringSerializationFormat, DateTimeFormatInfo.InvariantInfo ) );
                    break;

                case BsonType.DateTime:
                    bsonWriter.WriteDateTime( value.ToUnixTimeMilliseconds() );
                    break;

                default:
                    var message = string.Format( "'{0}' is not a valid DateTimeOffset representation.", _representation );
                    throw new BsonSerializationException( message );
            }
        }

        /// <summary></summary>
        public DateTimeOffsetSerializer WithRepresentation( BsonType representation )
        {
            if ( representation == _representation )
                return this;

            return new DateTimeOffsetSerializer( representation );
        }

        /// <summary></summary>
        IBsonSerializer IRepresentationConfigurable.WithRepresentation( BsonType representation )
        {
            return WithRepresentation( representation );
        }

        //protected Exception CreateCannotDeserializeFromBsonTypeException( BsonType bsonType )
        //{
        //    var message = string.Format( "Cannot deserialize a '{0}' from BsonType '{1}'.",
        //        BsonUtils.GetFriendlyTypeName( ValueType ),
        //        bsonType );
        //    return new FormatException( message );
        //}
    }

}