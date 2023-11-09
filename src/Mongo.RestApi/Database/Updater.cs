using Mongo.RestApi.ApiModels;
using Mongo.RestApi.ErrorHandling;
using Mongo.RestApi.Extensions;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.Json;

namespace Mongo.RestApi.Database
{
    public class Updater : IUpdater
    {
        private readonly IDatabaseProvider _databaseProvider;

        public Updater(IDatabaseProvider databaseProvider)
        {
            _databaseProvider = databaseProvider;
        }

        public async Task RunAsync(
            string connectionName,
            string databaseName,
            string collectionName,
            UpdateModel model,
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

        private static Command<BsonDocument> BuildCommand(string collectionName, UpdateModel model)
        {
            var command = JsonSerializer.Serialize(new
            {
                update = collectionName,
                updates = model.Updates.Select(x => new
                {
                    q = x.Q,
                    u = x.U,
                    upsert = x.Upsert,
                    multi = x.Multi,
                    arrayFilters = x.ArrayFilters ?? Array.Empty<dynamic>()
                })
            });
            return new JsonCommand<BsonDocument>(command);
        }
    }
}
