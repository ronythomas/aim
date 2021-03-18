using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;


namespace Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //setup IoC for DI
            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddScoped<IOrderService, OrderService>();

            services.AddHttpClient("aim_api", c => { c.BaseAddress = new Uri("https://widgetco.azurewebsites.net/api/"); });

            var serviceProvider = services.BuildServiceProvider();

            //configure logging
            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<Program>();
            logger.LogDebug("starting application");

            var service = serviceProvider.GetService<IOrderService>();
            var orderResponse = await service.GetOrder();

            logger.LogInformation($"ship date is {orderResponse.ShipDate}");
            logger.LogDebug("stopping application!");
        }
    }
}
