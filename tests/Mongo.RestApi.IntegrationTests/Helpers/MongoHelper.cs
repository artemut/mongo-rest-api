using Microsoft.Extensions.Configuration;
using Mongo.RestApi.IntegrationTests.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Mongo.RestApi.IntegrationTests.Helpers
{
    internal class MongoHelper
    {
        private readonly MongoClient _mongoClient;

        public MongoHelper()
        {
            var connString = Config.Value.GetConnectionString("db");
            var mongoSettings = MongoClientSettings.FromConnectionString(connString);
            _mongoClient = new MongoClient(mongoSettings);
        }

        public async Task InsertManyAsync<T>(
            string databaseName,
            string collectionName,
            T[] items)
        {
            var db = _mongoClient.GetDatabase(databaseName);
            var collection = db.GetCollection<BsonDocument>(collectionName);
            await collection.InsertManyAsync(items.ToBsonDocuments());
        }

        public async Task<T[]> LoadDocumentsAsync<T>(string databaseName, string collectionName)
        {
            var db = _mongoClient.GetDatabase(databaseName);
            var collection = db.GetCollection<BsonDocument>(collectionName);
            var documents = await collection.AsQueryable().ToListAsync();
            return documents.FromBsonDocuments<T>();
        }

        public async Task ClearAsync(string databaseName)
        {
            var db = _mongoClient.GetDatabase(databaseName);
            var cursor = await db.ListCollectionNamesAsync();
            while (await cursor.MoveNextAsync())
            {
                foreach (var collectionName in cursor.Current)
                {
                    var collection = db.GetCollection<BsonDocument>(collectionName);
                    await collection.DeleteManyAsync(FilterDefinition<BsonDocument>.Empty);
                }
            }
        }
    }
}
