using Mongo.RestApi.ApiModels;

namespace Mongo.RestApi.Database
{
    public interface IDeleter
    {
        Task RunAsync(
            string connectionName,
            string databaseName,
            string collectionName,
            DeleteModel model,
            CancellationToken token);
    }
}
