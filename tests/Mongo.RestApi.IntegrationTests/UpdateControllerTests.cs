using FluentAssertions;
using Mongo.RestApi.IntegrationTests.Extensions;
using Mongo.RestApi.IntegrationTests.Helpers;
using Mongo.RestApi.IntegrationTests.Models;
using System.Net;

namespace Mongo.RestApi.IntegrationTests
{
    public class UpdateControllerTests : IDisposable
    {
        private readonly MongoHelper _mongoHelper;

        public UpdateControllerTests()
        {
            _mongoHelper = new MongoHelper();
        }

        [Theory, ModelCustomization]
        public async Task Update_Should_DoNothing_If_NoMatchingDocuments_And_UpsertIsFalse(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var notExistingId = existingItems.Sum(x => x._id);
            var body = $@"{{ ""updates"": [ {{
                ""q"": {{ ""_id"": {notExistingId} }},
                ""u"": {{ ""$set"": {{ ""Name"": ""new_name"" }} }},
                ""upsert"": false
            }} ] }}";

            var response = await client.PostAsync(UrlBuilder.Build(collectionName, "update"), body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().BeEquivalentTo(existingItems);
        }

        [Theory, ModelCustomization]
        public async Task Update_Should_UpdateMatchingDocument_WithReplaceMode(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var matchingId = existingItems[1]._id;
            var body = $@"{{ ""updates"": [ {{
                ""q"": {{ ""_id"": {matchingId} }},
                ""u"": {{ ""Name"": ""new_name"", ""Array"": [ ""array_item"" ] }}
            }} ] }}";

            var response = await client.PostAsync(UrlBuilder.Build(collectionName, "update"), body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            existingItems[1].Name = "new_name";
            existingItems[1].Array = new[]{ "array_item" };
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().BeEquivalentTo(existingItems);
        }

        [Theory, ModelCustomization]
        public async Task Update_Should_UpdateMatchingDocument_WithUpdateMode(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var matchingId = existingItems[1]._id;
            var body = $@"{{ ""updates"": [ {{
                ""q"": {{ ""_id"": {matchingId} }},
                ""u"": {{ ""$push"": {{ ""Array"": ""new_array_item"" }} }}
            }} ] }}";

            var response = await client.PostAsync(UrlBuilder.Build(collectionName, "update"), body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            existingItems[1].Array = existingItems[1].Array!.Concat(new[] { "new_array_item" }).ToArray();
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().BeEquivalentTo(existingItems);
        }

        [Theory, ModelCustomization]
        public async Task Update_Should_AddNewDocument_If_NoMatchingDocuments_And_UpsertIsTrue(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var notExistingId = existingItems.Sum(x => x._id);
            var body = $@"{{ ""updates"": [ {{
                ""q"": {{ ""_id"": {notExistingId} }},
                ""u"": {{ ""Name"": ""new_name"", ""Array"": [ ""array_item"" ] }},
                ""upsert"": true
            }} ] }}";

            var response = await client.PostAsync(UrlBuilder.Build(collectionName, "update"), body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var expectedNewItem = new DataModel
            {
                _id = notExistingId,
                Name = "new_name",
                Array = new []{ "array_item" }
            };
            var expectedItems = existingItems.Concat(new[] { expectedNewItem }).ToArray();
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().BeEquivalentTo(expectedItems);
        }

        [Theory, ModelCustomization]
        public async Task Update_Should_UpdateMultipleMatchingDocuments(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var body = @$"{{ ""updates"": [ {{
                ""q"": {{ }},
                ""u"": {{ ""$push"": {{ ""Array"": ""new_array_item"" }} }},
                ""multi"": true
            }} ] }}";

            var response = await client.PostAsync(UrlBuilder.Build(collectionName, "update"), body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            foreach (var existingItem in existingItems)
            {
                existingItem.Array = existingItem.Array!.Concat(new[] { "new_array_item" }).ToArray();
            }
            var actualItems = await _mongoHelper.LoadDocumentsAsync<DataModel>(
                Constants.DatabaseName,
                collectionName);
            actualItems.Should().NotBeNull();
            actualItems.OrderBy(x => x._id).Should().BeEquivalentTo(existingItems.OrderBy(x => x._id));
        }

        [Theory, ModelCustomization]
        public async Task Update_Should_Return400_If_ConnectionNameIsUnknown(
            string connectionName,
            string collectionName,
            HttpClient client)
        {
            var body = @$"{{ ""updates"": [ {{
                ""q"": {{ }},
                ""u"": {{ ""_id"": 123 }},
                ""upsert"": true
            }} ] }}";

            var response = await client.PostAsync($"{connectionName}/{Constants.DatabaseName}/{collectionName}/update", body);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void Dispose()
        {
            _mongoHelper.ClearAsync(Constants.DatabaseName).GetAwaiter().GetResult();
        }
    }
}
