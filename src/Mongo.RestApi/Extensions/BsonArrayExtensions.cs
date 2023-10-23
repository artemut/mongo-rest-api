using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.RestApi.Extensions
{
    internal static class BsonArrayExtensions
    {
        public static IEnumerable<dynamic> Extract(this BsonArray bsonArray)
        {
            return bsonArray.Select(x => BsonSerializer.Deserialize<dynamic>(x.AsBsonDocument));
        }
    }
}
