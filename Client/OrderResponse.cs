using System;
using System.Text.Json.Serialization;

namespace Client
{
    public class Order
    {
        [JsonPropertyName("shipDate")]
        public DateTime? ShipDate { get; set; }
    }
}