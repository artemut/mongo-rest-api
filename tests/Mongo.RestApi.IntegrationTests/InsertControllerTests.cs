using FluentAssertions;
using Mongo.RestApi.IntegrationTests.Extensions;
using Mongo.RestApi.IntegrationTests.Helpers;
using Mongo.RestApi.IntegrationTests.Models;
using System.Net;

namespace Mongo.RestApi.IntegrationTests
{
    public class InsertControllerTests : IDisposable
    {
        private readonly MongoHelper _mongoHelper;

        public InsertControllerTests()
        {
            _mongoHelper = new MongoHelper();
        }

        [Theory, ModelCustomization]
        public async Task Insert_Should_CreateExpectedDocuments(
            string collectionName,
            DataModel[] items,
            HttpClient client)
        {
            var body = new { documents = items };

            var response = await client.PostAsync($"/{Constants.DatabaseName}/{collectionName}/insert", body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().BeEquivalentTo(items);
        }

        [Theory, ModelCustomization]
        public async Task Insert_Should_NotInsertNewDocuments_If_ADocumentWithGivenIdAlreadyExists(
            string collectionName,
            DataModel[] items,
            HttpClient client)
        {
            var existingItems = new[] { items[0] };
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var body = new { documents = items };

            var response = await client.PostAsync($"/{Constants.DatabaseName}/{collectionName}/insert", body);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().BeEquivalentTo(existingItems);
        }

        public void Dispose()
        {
            _mongoHelper.ClearAsync(Constants.DatabaseName).GetAwaiter().GetResult();
        }
    }
}
