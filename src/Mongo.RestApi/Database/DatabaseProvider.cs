using MongoDB.Driver;
using System.Collections.Concurrent;

namespace Mongo.RestApi.Database
{
    public class DatabaseProvider : IDatabaseProvider
    {
        private readonly ConcurrentDictionary<string, Lazy<IMongoDatabase>> _databases = new();
        private readonly MongoClient _mongoClient;

        public DatabaseProvider(IConfiguration config)
        {
            var connString = config.GetConnectionString("MongoConnection");
            var mongoSettings = MongoClientSettings.FromConnectionString(connString);
            _mongoClient = new MongoClient(mongoSettings);
        }

        public IMongoDatabase GetDatabase(string databaseName)
        {
            return _databases.GetOrAdd(
                    databaseName,
                    key => new Lazy<IMongoDatabase>(() => _mongoClient.GetDatabase(key)))
                .Value;
        }
    }
}
