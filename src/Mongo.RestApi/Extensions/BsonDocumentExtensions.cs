using MongoDB.Bson;

namespace Mongo.RestApi.Extensions
{
    internal static class BsonDocumentExtensions
    {
        public static dynamic[]? GetWriteErrors(this BsonDocument document)
        {
            if (!document.Contains("writeErrors"))
            {
                return null;
            }
            var writeErrors = document["writeErrors"].AsBsonArray;
            return writeErrors?.Extract().ToArray();
        }
    }
}
