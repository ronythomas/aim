using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Client
{
    public interface IOrderService
    {
        Task<ApiResponse<Order>> GetOrder();
    }

    public class OrderService : IOrderService
    {
        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<OrderService> logger;

        public OrderService(IHttpClientFactory httpClientFactory, ILogger<OrderService> logger)
        {
            this.httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResponse<Order>> GetOrder()
        {
            var httpClient = httpClientFactory.CreateClient("aim_api");
            logger.LogDebug("calling API");

            try
            {
                var response = await httpClient.GetAsync("service");

                if (response.IsSuccessStatusCode)
                {
                    logger.LogDebug("API call succeeded");
                    var responseString = await response.Content.ReadAsStringAsync();

                    if (string.IsNullOrEmpty(responseString))
                    {
                        return new ApiResponse<Order>(false, null);
                    }

                    var order = JsonSerializer.Deserialize<Order>(responseString);
                    return new ApiResponse<Order>(order?.ShipDate != null, order);
                }

                var message = response.ReasonPhrase;
                logger.LogCritical(message);
                return new ApiResponse<Order>(false, null, message);

            }
            catch (Exception exception)
            {
                logger.LogError(exception, "exception on communicating with API");
                return new ApiResponse<Order>(false, null, exception.Message);
            }
        }
    }
}
