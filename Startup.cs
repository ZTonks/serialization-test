using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

[assembly: FunctionsStartup(typeof(serialization_test.Startup))]
namespace serialization_test
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.Configure<JsonSerializerOptions>(o => { });
        }
    }
}
