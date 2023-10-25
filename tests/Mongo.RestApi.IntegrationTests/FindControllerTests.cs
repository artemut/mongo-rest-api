using FluentAssertions;
using Mongo.RestApi.IntegrationTests.Extensions;
using Mongo.RestApi.IntegrationTests.Helpers;
using Mongo.RestApi.IntegrationTests.Models;
using System.Net;

namespace Mongo.RestApi.IntegrationTests
{
    public class FindControllerTests : IDisposable
    {
        private readonly MongoHelper _mongoHelper;

        public FindControllerTests()
        {
            _mongoHelper = new MongoHelper();
        }

        [Theory, ModelCustomization]
        public async Task Find_Should_ReturnEmptyArray_If_NoData(
            string databaseName,
            string collectionName,
            HttpClient client)
        {
            var body = new { filter = new { } };

            var response = await client.PostAsync($"/{databaseName}/{collectionName}/find", body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var array = await response.ReadAsAsync<dynamic[]>();
            array.Should().BeEmpty();
        }

        [Theory, ModelCustomization]
        public async Task Find_Should_ReturnEmptyArray_If_FilterDoesNotMatchAnyDocument(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var body = new { filter = new { not_existing_property = "foo" } };

            var response = await client.PostAsync($"/{Constants.DatabaseName}/{collectionName}/find", body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var array = await response.ReadAsAsync<DataModel[]>();
            array.Should().BeEmpty();
        }

        [Theory, ModelCustomization]
        public async Task Find_Should_ReturnAllItems_If_FilterMatchesAllDocuments(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var body = new { filter = new { } };

            var response = await client.PostAsync($"/{Constants.DatabaseName}/{collectionName}/find", body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var array = await response.ReadAsAsync<DataModel[]>();
            array.Should().BeEquivalentTo(existingItems);
        }

        [Theory, ModelCustomization]
        public async Task Find_Should_ReturnExpectedItems_If_FilterMatchesSomeDocuments(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var expectedItems = new[] { existingItems[0], existingItems[2] };
            var id0 = expectedItems[0]._id;
            var id1 = expectedItems[1]._id;
            var body = $@"{{ ""filter"": {{ ""$or"": [{{ ""_id"": {id0} }},{{ ""_id"": {id1} }}] }} }}";

            var response = await client.PostAsync($"/{Constants.DatabaseName}/{collectionName}/find", body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var array = await response.ReadAsAsync<DataModel[]>();
            array.Should().BeEquivalentTo(expectedItems);
        }

        [Theory]
        [InlineModelCustomization(true)]
        [InlineModelCustomization(false)]
        public async Task Find_Should_ReturnExpectedItems_WithCorrectSorting(
            bool ascending,
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var expectedItems = ascending
                ? existingItems.OrderBy(x => x.Name).ToArray()
                : existingItems.OrderByDescending(x => x.Name).ToArray();
            var body = new { filter = new { }, sort = new { Name = ascending ? 1 : -1 } };

            var response = await client.PostAsync($"/{Constants.DatabaseName}/{collectionName}/find", body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var array = await response.ReadAsAsync<DataModel[]>();
            array.Should().BeEquivalentTo(expectedItems, x => x.WithStrictOrdering());
        }

        [Theory, ModelCustomization]
        public async Task Find_Should_ReturnExpectedItems_WithCorrectProjection(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var body = new { filter = new { }, projection = new { Name = 1 } };

            var response = await client.PostAsync($"/{Constants.DatabaseName}/{collectionName}/find", body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var array = await response.ReadAsAsync<DataModel[]>();
            array.Should().BeEquivalentTo(existingItems, o => o.Including(x => x._id).Including(x => x.Name));
            array.Should().AllSatisfy(x => x.Array.Should().BeNull());
        }

        [Theory, ModelCustomization]
        public async Task Find_Should_ReturnExpectedItems_WithSkipAndLimit(
            string collectionName,
            DataModel[] existingItems,
            HttpClient client)
        {
            await _mongoHelper.InsertManyAsync(Constants.DatabaseName, collectionName, existingItems);
            var expectedItems = existingItems.Skip(1).Take(1).ToArray();
            var body = new { filter = new { }, skip = 1, limit = 1 };

            var response = await client.PostAsync($"/{Constants.DatabaseName}/{collectionName}/find", body);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var array = await response.ReadAsAsync<DataModel[]>();
            array.Should().BeEquivalentTo(expectedItems);
        }

        public void Dispose()
        {
            _mongoHelper.ClearAsync(Constants.DatabaseName).GetAwaiter().GetResult();
        }
    }
}
