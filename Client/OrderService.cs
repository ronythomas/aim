using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Client
{
    public interface IOrderService
    {
        Task<OrderResponse> GetOrder();
    }

    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory clientFactory;
        private readonly ILogger<OrderService> logger;

        public OrderService(IHttpClientFactory clientFactory, ILogger<OrderService> logger)
        {
            this.clientFactory = clientFactory;
            this.logger = logger;
        }

        public async Task<OrderResponse> GetOrder()
        {
            try
            {
                var httpClient = clientFactory.CreateClient("aim_api");

                logger.LogDebug("calling API");
                var response = await httpClient.GetAsync("service");

                if (response.IsSuccessStatusCode)
                {
                    logger.LogDebug("API call succeeded");
                    var responseString = await response.Content.ReadAsStringAsync();

                    //fix invalid json response from API
                    //delete once API is fixed
                    responseString = responseString.Replace("\"id\": 0", "\"id\": 0,");
                    responseString = responseString.Replace("\"petId\": 0", "\"petId\": 0,");

                    var orderResponse = JsonSerializer.Deserialize<OrderResponse>(responseString);
                    return orderResponse;
                }
                else
                {
                    var message = await response.Content.ReadAsStringAsync();
                    logger.LogError(message);

                    throw new HttpRequestException(message);
                }
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "exception on communicating with API");
                throw;
            }
        }
    }
}
