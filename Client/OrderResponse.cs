using System;
using System.Text.Json.Serialization;

namespace Client
{
    public class OrderResponse
    {
        [JsonPropertyName("shipDate")]
        public DateTime ShipDate { get; set; }
    }
}