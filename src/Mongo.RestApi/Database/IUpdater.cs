using Mongo.RestApi.ApiModels;

namespace Mongo.RestApi.Database
{
    public interface IUpdater
    {
        Task RunAsync(
            string databaseName,
            string collectionName,
            UpdateModel model,
            CancellationToken token);
    }
}
