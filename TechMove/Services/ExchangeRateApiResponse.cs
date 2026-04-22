using System.Text.Json.Serialization;

namespace TechMove.Services
{
    public class ExchangeRateApiResponse
    {
        [JsonPropertyName("rates")]
        public Dictionary<String, decimal>? Rates { get; set; }
        
    }
}
