using System;
using System.Text.Json;
using System.Threading.Tasks;
using Enable.Extensions.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace serialization_test
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var isDevelopment = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development";

            if (isDevelopment)
            {
                configuration.ApplyDevelopmentOverrides();
            }

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(serviceCollection =>
                {
                    serviceCollection
                        .Configure<JsonSerializerOptions>(options =>
                        {
                        });
                })
                .Build();

            await host.RunAsync();
        }
    }
}
