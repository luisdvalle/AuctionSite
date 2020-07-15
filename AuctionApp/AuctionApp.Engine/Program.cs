using System.Threading.Tasks;
using AuctionApp.Common.Services;
using AuctionApp.Common.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace AuctionApp.Engine
{
    class Program
    {
        static async Task Main()
        {
            var builder = new HostBuilder();

            builder.ConfigureWebJobs(b =>
            {
                b.AddAzureStorageCoreServices();
                b.AddAzureStorage();
                b.AddTimers();
                b.Services.AddSingleton<IRepository, Repository>(provider =>
                    new Repository("DefaultEndpointsProtocol=https;AccountName=eddappauctionsite;AccountKey=hRwYFEBoOlI8OaFH4EJoBK3bhs9esT2fuGlxz2N4daNjIUEBDW3tifziyYO1BSg5mjXelA6lLJh8VrJOkXhgOg==;EndpointSuffix=core.windows.net"));
                b.Services.AddScoped<IAuctionService, AuctionService>();
            });
            builder.ConfigureLogging((context, b) =>
            {
                b.AddConsole();
            });

            var host = builder.Build();
            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
