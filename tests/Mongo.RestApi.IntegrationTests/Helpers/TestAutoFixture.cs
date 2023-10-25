using AutoFixture;
using AutoFixture.AutoMoq;

namespace Mongo.RestApi.IntegrationTests.Helpers
{
    internal class TestAutoFixture : Fixture
    {
        public TestAutoFixture()
        {
            Customize(new CompositeCustomization(
                new AutoMoqCustomization(),
                new HttpClientCustomization()));
        }

        private class HttpClientCustomization : ICustomization
        {
            public void Customize(IFixture fixture)
            {
                fixture.Register(
                    () =>
                    {
                        var application = new SelfHostedApi();
                        return application.CreateClient();
                    });
            }
        }
    }
}
