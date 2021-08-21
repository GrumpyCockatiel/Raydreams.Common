using System;
using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;

namespace Raydreams.Common.Serializers
{
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