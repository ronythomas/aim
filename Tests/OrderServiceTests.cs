using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Contrib.HttpClient;
using NUnit.Framework;

namespace Tests
{
    public class OrderServiceTests
    {
        [Test]
        public void OrderServiceShouldThrowForMissingArgs()
        {
            var serviceProvider = GetServiceProvider();
            var httpClientFactory = serviceProvider.GetService<IHttpClientFactory>();
            var logger = serviceProvider.GetService<ILogger<OrderService>>();

            var exception = Assert.Throws<ArgumentNullException>(() => new OrderService(null, logger));
            Assert.IsNotNull(exception, "exception is null");
            Assert.AreEqual("httpClientFactory", exception.ParamName, "httpClientFactory");

            exception = Assert.Throws<ArgumentNullException>(() => new OrderService(httpClientFactory, null));
            Assert.IsNotNull(exception, "exception is null");
            Assert.AreEqual("logger", exception.ParamName, "logger");

            Assert.DoesNotThrow(() => new OrderService(httpClientFactory, logger));
        }

        [Test]
        public async Task GetOrder()
        {
            var serviceProvider = GetServiceProvider();
            var orderService = serviceProvider.GetService<IOrderService>();
            var response = await orderService.GetOrder();

            Assert.IsNotNull(response, "response is null");
            Assert.IsTrue(response.IsSuccess, "api call should result in success");
            Assert.IsNotNull(response.Result, "response.Result is null");
            Assert.IsNotNull(response.Result.ShipDate, "response.ShipDate is null");
            Assert.IsNotNull(response.Result.ShipDate, "response.ShipDate is not valid");
            Assert.IsNull(response.ErrorMessage, "response.ErrorMessage should be null");
        }

        [Test]
        public async Task GetOrderInvalidEndpoint()
        {
            var serviceProvider = GetServiceProviderMsgHandlerInvalidEndpoint();
            var orderService = serviceProvider.GetService<IOrderService>();
            var response = await orderService.GetOrder();

            Assert.IsNotNull(response, "response is null");
            Assert.IsFalse(response.IsSuccess, "api call should not result in success");
            Assert.IsNull(response.Result, "response.Result should be null");
            Assert.IsNotNull(response.ErrorMessage, "response.ErrorMessage should be null");
        }

        [Test]
        public async Task GetOrderMissingShippingDate()
        {
            var apiResponse = "{\"id\": 0, \"petId\": 0, \"quantity\": 0, \"status\": \"placed\",\"complete\": true }";
            var serviceProvider = GetServiceProviderMsgHandlerInvalidResponse(apiResponse);
            var orderService = serviceProvider.GetService<IOrderService>();
            var response = await orderService.GetOrder();

            Assert.IsNotNull(response, "response is null");
            Assert.IsFalse(response.IsSuccess, "api call should not result in success");
            Assert.IsNull(response.Result.ShipDate, "response.Result.ShipDate should be null");
        }

        [Test]
        public async Task GetOrderInvalidResponse()
        {
            var serviceProvider = GetServiceProviderMsgHandlerInvalidResponse(string.Empty);
            var orderService = serviceProvider.GetService<IOrderService>();
            var response = await orderService.GetOrder();

            Assert.IsNotNull(response, "response is null");
            Assert.IsFalse(response.IsSuccess, "api call should not result in success");
            Assert.IsNull(response.Result, "response.Result should be null");
        }

        [Test]
        public async Task GetOrderInvalidShipDate()
        {
            var apiResponse = "{\"id\": 0, \"petId\": 0, \"quantity\": 0, \"shipDate\": \"2021-03-1617:59:01.901Z\", \"status\": \"placed\",\"complete\": true }";
            var serviceProvider = GetServiceProviderMsgHandlerInvalidResponse(apiResponse);
            var orderService = serviceProvider.GetService<IOrderService>();
            var response = await orderService.GetOrder();

            Assert.IsNotNull(response, "response is null");
            Assert.IsFalse(response.IsSuccess, "api call should not result in success");
            Assert.IsNull(response.Result, "response.Result should be null");
        }

        [Test]
        public async Task GetOrderNonSuccessStatusCode()
        {
            var apiResponse = "{\"id\": 0, \"petId\": 0, \"quantity\": 0, \"shipDate\": \"2021-03-1617:59:01.901Z\", \"status\": \"placed\",\"complete\": true }";
            var serviceProvider = GetServiceProviderMsgHandlerInvalidResponse(apiResponse, HttpStatusCode.ServiceUnavailable);
            var orderService = serviceProvider.GetService<IOrderService>();
            var response = await orderService.GetOrder();

            Assert.IsNotNull(response, "response is null");
            Assert.IsFalse(response.IsSuccess, "api call should not result in success");
            Assert.IsNull(response.Result, "response.Result should be null");
        }

        [Test]
        public async Task GetOrderMsgHandlerThrowsException()
        {
            var serviceProvider = GetServiceProviderMsgHandlerThatThrowsException();
            var orderService = serviceProvider.GetService<IOrderService>();
            var response = await orderService.GetOrder();

            Assert.IsNotNull(response, "response is null");
            Assert.IsFalse(response.IsSuccess, "api call should not result in success");
            Assert.IsNull(response.Result, "response.Result should be null");
            Assert.IsNotNull(response.ErrorMessage, "response.ErrorMessage should be null");
        }

        private ServiceProvider GetServiceProvider()
        {
            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddScoped<IOrderService, OrderService>();

            services.AddHttpClient("aim_api", c => { c.BaseAddress = new Uri("https://widgetco.azurewebsites.net/api/"); });

            return services.BuildServiceProvider();
        }

        private ServiceProvider GetServiceProviderMsgHandlerInvalidEndpoint()
        {
            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddScoped<IOrderService, OrderService>();

            services.AddHttpClient("aim_api", c => { c.BaseAddress = new Uri("https://widgetco.azurewebsites.net/invalid-endpoint"); });

            return services.BuildServiceProvider();
        }

        private ServiceProvider GetServiceProviderMsgHandlerInvalidResponse(string response, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, "https://widgetco.azurewebsites.net/api/service")
                .ReturnsResponse(statusCode, response, "application/json");

            var factory = handler.CreateClientFactory();

            Mock.Get(factory).Setup(x => x.CreateClient("aim_api"))
            .Returns(() =>
            {
                var client = handler.CreateClient();
                client.BaseAddress = new Uri("https://widgetco.azurewebsites.net/api/");
                return client;
            });

            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddScoped<IOrderService>(sp => new OrderService(factory, sp.GetService<ILogger<OrderService>>()));

            return services.BuildServiceProvider();
        }

        private ServiceProvider GetServiceProviderMsgHandlerThatThrowsException()
        {
            var handler = new Mock<HttpMessageHandler>();
            handler.SetupRequest(HttpMethod.Get, "https://widgetco.azurewebsites.net/api/service")
            .Throws<HttpRequestException>();

            var factory = handler.CreateClientFactory();

            Mock.Get(factory).Setup(x => x.CreateClient("aim_api"))
            .Returns(() =>
            {
                var client = handler.CreateClient();
                client.BaseAddress = new Uri("https://widgetco.azurewebsites.net/api/");
                return client;
            });

            var services = new ServiceCollection()
                .AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Debug))
                .AddScoped<IOrderService>(sp => new OrderService(factory, sp.GetService<ILogger<OrderService>>()));

            return services.BuildServiceProvider();
        }
    }
}