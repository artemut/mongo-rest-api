using Mongo.RestApi.ApiModels;

namespace Mongo.RestApi.Database
{
    public interface IDeleter
    {
        Task RunAsync(
            string databaseName,
            string collectionName,
            DeleteModel model,
            CancellationToken token);
    }
}
