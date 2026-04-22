using System.Net.Http.Json;
using Microsoft.Extensions.Options;
using TechMove.Models;

namespace TechMove.Services
{
    // Implementation that communicates with an external API for exchange rates
    public class ExchangeRateService : IExchangeRateServices
    {
        private readonly HttpClient _httpClient;
        private readonly ExchangeRateApiOptions _options;

        // Constructor handles the injection of HttpClient and configuration options
        public ExchangeRateService(HttpClient httpClient, IOptions<ExchangeRateApiOptions> options)
        {
            _httpClient = httpClient;
            _options = options.Value;
        }

        // Fetches the latest USD to ZAR rate from the configured API endpoint
        public async Task<decimal> GetUsdToZarRateAsync(CancellationToken cancellationToken = default)
        {
            // Ensure the API URL is configured in appsettings.json
            if (String.IsNullOrWhiteSpace(_options.LatestUsdUrl))
                throw new InvalidOperationException("ExchangeRateApi:LatestUsdUrl is not configured.");

            // Perform the asynchronous GET request and deserialize the JSON response
            var response = await _httpClient.GetFromJsonAsync<ExchangeRateApiResponse>(
                _options.LatestUsdUrl,
                cancellationToken
                );

            // Ensure we got a valid rate back from the API
            if (response?.Rates is null || !response.Rates.TryGetValue("ZAR", out var zarPerUsd) || zarPerUsd <= 0)
            {
                throw new InvalidOperationException("Could not read USD to ZAR rate from the exchange API response.");
            }

            return zarPerUsd;
        }
    }
}
