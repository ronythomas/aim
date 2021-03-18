using System;
using System.Threading.Tasks;
using Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Tests
{
    public class OrderServiceTests
    {
        IOrderService orderService;
        IServiceCollection services;
        ServiceProvider serviceProvider;

        public OrderServiceTests()
        {

            services = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddScoped<IOrderService, OrderService>();

            services.AddHttpClient("aim_api", c => { c.BaseAddress = new Uri("https://widgetco.azurewebsites.net/api/"); });
            serviceProvider = services.BuildServiceProvider();
            orderService = serviceProvider.GetService<IOrderService>();
        }

        [Test]
        public async Task GetOrder()
        {
            var response = await orderService.GetOrder();

            Assert.IsNotNull(response, "response is null");
            Assert.IsNotNull(response.ShipDate, "response.ShipDate is null");
        }
    }
}