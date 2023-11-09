using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace Mongo.RestApi.IntegrationTests.Helpers
{
    internal sealed class SelfHostedApi : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            return base.CreateHost(builder);
        }
    }
}
