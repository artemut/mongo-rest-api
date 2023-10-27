using Mongo.RestApi.ApiModels;
using Mongo.RestApi.ErrorHandling;
using Mongo.RestApi.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace Mongo.RestApi.Database
{
    public class Deleter : IDeleter
    {
        private readonly IDatabaseProvider _databaseProvider;

        public Deleter(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        public async Task RunAsync(
            string connectionName,
            string databaseName,
            string collectionName,
            DeleteModel model,
            CancellationToken token)
        {
            var database = _databaseProvider.GetDatabase(connectionName, databaseName);

            var command = BuildCommand(collectionName, model);
            var result = await database.RunCommandAsync(command, cancellationToken: token);

            var writeErrors = result.GetWriteErrors();
            if (writeErrors != null && writeErrors.Any())
            {
                throw new CommandException("Command result indicates error:" +
                                           $"{Environment.NewLine}" +
                                           $"{JsonSerializer.Serialize(writeErrors)}");
            }
        }

        private static Command<BsonDocument> BuildCommand(string collectionName, DeleteModel model)
        {
            var command = JsonSerializer.Serialize(new
            {
                delete = collectionName,
                deletes = model.Deletes.Select(x => new
                {
                    q = x.Q,
                    limit = x.Limit
                })
            });
            return new JsonCommand<BsonDocument>(command);
        }
    }
}
