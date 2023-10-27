namespace Mongo.RestApi.IntegrationTests.Helpers
{
    internal static class UrlBuilder
    {
        public static string Build(string collectionName, string endpoint)
        {
            return $"{Constants.ConnectionName}/{Constants.DatabaseName}/{collectionName}/{endpoint}";
        }
    }
}
