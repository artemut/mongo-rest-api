using Mongo.RestApi.ErrorHandling;
using MongoDB.Driver;
using System.Collections.Concurrent;

namespace Mongo.RestApi.Database
{
    public class DatabaseProvider : IDatabaseProvider
    {
        private readonly IConfiguration _config;
        private readonly ConcurrentDictionary<string, Lazy<IMongoClient>> _mongoClients = new();
        private readonly ConcurrentDictionary<string, Lazy<IMongoDatabase>> _databases = new();

        public DatabaseProvider(IConfiguration config)
        {
            _config = config;
        }

        public IMongoDatabase GetDatabase(string connectionName, string databaseName)
        {
            var mongoClient = GetMongoClient(connectionName);

            return _databases.GetOrAdd(
                    databaseName,
                    key => new Lazy<IMongoDatabase>(() => mongoClient.GetDatabase(key)))
                .Value;
        }

        private IMongoClient GetMongoClient(string connectionName)
        {
            var connString = _config.GetConnectionString(connectionName);
            if (string.IsNullOrWhiteSpace(connString))
            {
                throw new ConnectionNotFoundException(
                    $"Connection string is not configured for '{connectionName}'");
            }

            return _mongoClients.GetOrAdd(
                    connectionName,
                    _ => new Lazy<IMongoClient>(() =>
                    {
                        var mongoSettings = MongoClientSettings.FromConnectionString(connString);
                        return new MongoClient(mongoSettings);
                    }))
                .Value;
        }
    }
}
