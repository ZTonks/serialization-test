using System;
using System.Text.Json;
using System.Threading.Tasks;
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

            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults(c =>
                {
                    c.Serializer = new MySerializer();
                })
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
