using AutoFixture.Xunit2;

namespace Mongo.RestApi.IntegrationTests.Helpers
{
    public class ModelCustomizationAttribute : AutoDataAttribute
    {
        public ModelCustomizationAttribute()
            : base(() => new TestAutoFixture())
        {
        }
    }
}
