using Newtonsoft.Json;
using System.Text;

namespace Mongo.RestApi.IntegrationTests.Extensions
{
    internal static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsync(
            this HttpClient client,
            string url,
            dynamic body)
        {
            return PostAsync(client, url, JsonConvert.SerializeObject(body));
        }

        public static Task<HttpResponseMessage> PostAsync(
            this HttpClient client,
            string url,
            string json)
        {
            var content = new StringContent(
                json,
                Encoding.UTF8,
                "application/json");
            return client.PostAsync(url, content);
        }
    }
}
