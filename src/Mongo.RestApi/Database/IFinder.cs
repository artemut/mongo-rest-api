using Mongo.RestApi.ApiModels;

namespace Mongo.RestApi.Database
{
    public interface IFinder
    {
        Task<List<dynamic>> RunAsync(
            string databaseName,
            string collectionName,
            FindModel model,
            CancellationToken token);
    }
}
