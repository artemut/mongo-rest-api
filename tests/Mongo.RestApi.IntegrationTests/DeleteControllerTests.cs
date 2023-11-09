using FluentAssertions;
using Mongo.RestApi.IntegrationTests.Extensions;
using Mongo.RestApi.IntegrationTests.Helpers;
using Mongo.RestApi.IntegrationTests.Models;
using System.Net;

namespace Mongo.RestApi.IntegrationTests
{
    public class DeleteControllerTests : IDisposable
    {
        private readonly MongoHelper _mongoHelper;

        public DeleteControllerTests()
        {
            _mongoHelper = new MongoHelper();
        }

        [Theory, ModelCustomization]
        public async Task Delete_Should_DoNothing_If_NoMatchingDocuments(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var notExistingId = existingItems.Sum(x => x._id);
            var body = new { deletes = new[] { new { q = new { _id = notExistingId } } } };

            var response = await client.PostAsync(UrlBuilder.Build(collectionName, "delete"), body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().BeEquivalentTo(existingItems);
        }

        [Theory, ModelCustomization]
        public async Task Delete_Should_DeleteMatchingDocuments(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var matchingId = existingItems[1]._id;
            var expectedRemainingItems = new[] { existingItems[0], existingItems[2] };
            var body = new { deletes = new[] { new { q = new { _id = matchingId } } } };

            var response = await client.PostAsync(UrlBuilder.Build(collectionName, "delete"), body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().BeEquivalentTo(expectedRemainingItems);
        }

        [Theory, ModelCustomization]
        public async Task Delete_Should_Return400_If_ConnectionNameIsUnknown(
            string connectionName,
            string collectionName,
            HttpClient client)
        {
            var body = new { deletes = Array.Empty<dynamic>() };

            var response = await client.PostAsync($"{connectionName}/{Constants.DatabaseName}/{collectionName}/delete", body);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void Dispose()
        {
            _mongoHelper.ClearAsync(Constants.DatabaseName).GetAwaiter().GetResult();
        }
    }
}
