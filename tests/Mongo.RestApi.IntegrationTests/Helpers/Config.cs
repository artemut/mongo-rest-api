using Microsoft.Extensions.Configuration;

namespace Mongo.RestApi.IntegrationTests.Helpers
{
    internal static class Config
    {
        static Config()
        {
            Value = new ConfigurationBuilder()
                .AddJsonFile("settings.json")
                .Build();
        }

        public static IConfiguration Value { get; }
    }
}
