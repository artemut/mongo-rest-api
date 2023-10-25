using Newtonsoft.Json;

namespace Mongo.RestApi.IntegrationTests.Extensions
{
    internal static class HttpResponseMessageExtensions
    {
        public static async Task<T?> ReadAsAsync<T>(this HttpResponseMessage message)
        {
            var content = await message.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
