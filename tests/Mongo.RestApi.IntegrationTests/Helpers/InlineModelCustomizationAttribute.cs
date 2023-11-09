using AutoFixture.Xunit2;

namespace Mongo.RestApi.IntegrationTests.Helpers
{
    public class InlineModelCustomizationAttribute : InlineAutoDataAttribute
    {
        public InlineModelCustomizationAttribute(params object[] values)
            : base(new ModelCustomizationAttribute(), values)
        {
        }
    }
}
