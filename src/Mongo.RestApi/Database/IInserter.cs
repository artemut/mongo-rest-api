using Mongo.RestApi.ApiModels;

namespace Mongo.RestApi.Database
{
    public interface IInserter
    {
        Task RunAsync(
            string databaseName,
            string collectionName,
            InsertModel model,
            CancellationToken token);
    }
}
