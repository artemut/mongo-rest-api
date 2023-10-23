using Mongo.RestApi.ApiModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Text.Json;

namespace Mongo.RestApi.Database
{
    public class Finder : IFinder
    {
        private readonly IDatabaseProvider _databaseProvider;

        public Finder(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        public async Task<List<dynamic>> RunAsync(
            string databaseName,
            string collectionName,
            FindModel model,
            CancellationToken token)
        {
            var database = _databaseProvider.GetDatabase(databaseName);

            var documents = new List<dynamic>();
            var batchNumber = 0;
            var cursorId = 0L;
            do
            {
                var command = ++batchNumber == 1
                    ? BuildFindCommand(collectionName, model)
                    : BuildGetMoreCommand(cursorId, collectionName);

                var result = await database.RunCommandAsync(command, cancellationToken: token);
                if (result["ok"].ToInt32() != 1)
                {
                    throw new Exception($"Command result indicates error. Batch number: {batchNumber}");
                }

                var cursor = result["cursor"];
                cursorId = cursor["id"].AsInt64;
                documents.AddRange(
                    ExtractDocuments(cursor, batchNumber == 1 ? "firstBatch" : "nextBatch"));

            } while (cursorId > 0);

            return documents;
        }

        private static Command<BsonDocument> BuildFindCommand(string collectionName, FindModel model)
        {
            var command = JsonSerializer.Serialize(new
            {
                find = collectionName,
                filter = model.Filter,
                sort = model.Sort ?? new { },
                projection = model.Projection ?? new { },
                skip = model.Skip ?? 0,
                limit = model.Limit ?? 0
            });
            return new JsonCommand<BsonDocument>(command);
        }

        private static Command<BsonDocument> BuildGetMoreCommand(long cursorId, string collectionName)
        {
            var command = JsonSerializer.Serialize(new
            {
                getMore = cursorId,
                collection = collectionName
            });
            return new JsonCommand<BsonDocument>(command);
        }

        private static IEnumerable<dynamic> ExtractDocuments(BsonValue cursor, string batchKey)
        {
            return cursor[batchKey]
                .AsBsonArray
                .Select(x => BsonSerializer.Deserialize<dynamic>(x.AsBsonDocument));
        }
    }
}
