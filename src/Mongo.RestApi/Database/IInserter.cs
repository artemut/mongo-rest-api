using Mongo.RestApi.ApiModels;

namespace Mongo.RestApi.Database
{
    public interface IInserter
    {
        Task RunAsync(
            string connectionName,
            string databaseName,
            string collectionName,
            InsertModel model,
            CancellationToken token);
    }
}
