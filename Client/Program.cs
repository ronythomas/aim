using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
            try
            {
                var service = serviceProvider.GetService<IOrderService>();
                var response = await service.GetOrder();

                if (response.IsSuccess)
                {
                    logger.LogInformation($"ship date is {response.Result.ShipDate}");
                }
                else
                {
                    logger.LogError($"unable to find ship date. reason: {response.ErrorMessage}");
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "error while accessing order service");
            }

            logger.LogDebug("stopping application!");
        }
    }
}
