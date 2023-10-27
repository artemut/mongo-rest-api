using MongoDB.Driver;

namespace Mongo.RestApi.Database
{
    public interface IDatabaseProvider
    {
        IMongoDatabase GetDatabase(string connectionName, string databaseName);
    }
}
