using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace Mongo.RestApi.IntegrationTests.Extensions
{
    internal static class BsonDocumentExtensions
    {
        public static BsonDocument[] ToBsonDocuments<T>(this IEnumerable<T> items)
        {
            return items.Select(x => x.ToBsonDocument()).ToArray();
        }

        public static T[] FromBsonDocuments<T>(this IEnumerable<BsonDocument> items)
        {
            return items.Select(x => BsonSerializer.Deserialize<T>(x)).ToArray();
        }
    }
}
