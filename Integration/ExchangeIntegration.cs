using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Canela.Models;

namespace Canela.Integration.Exchange
{
    public class ExchangeIntegration
    {
        private readonly string _apiKey;
        private readonly string _apiUrl;
        private readonly string _apiHost;

        private readonly ILogger<ExchangeIntegration> _logger;
        public ExchangeIntegration(ILogger<ExchangeIntegration> logger, IConfiguration configuration)
        {
            _logger = logger;
            _apiKey = configuration["Exchange:ApiKey"];
            _apiUrl = configuration["Exchange:ApiUrl"];
            _apiHost = configuration["Exchange:ApiHost"];
        }

        public async Task<double> GetExchangeRate(TipoCambio tipoCambio)
        {
            return await GetExchangeRate(tipoCambio.From, tipoCambio.To, 1);
        }

        public async Task<double> GetExchangeRate(string from, string to, int q)
        {
            _logger.LogInformation("Fetching exchange rate from {0} to {1} for amount {2}", from, to, q);
            double exchangeRate = 0.0;
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add("x-rapidapi-key", _apiKey);
                httpClient.DefaultRequestHeaders.Add("Accept", "application/json");
                httpClient.DefaultRequestHeaders.Add("x-rapidapi-host", _apiHost);
                var url = $"{_apiUrl}/exchange?from={from}&to={to}&q={q}";
                _logger.LogInformation($"Request URL: {url}");
                var response = await httpClient.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    _logger.LogInformation("Successfully fetched exchange rate from {0} to {1}", from, to);
                    _logger.LogInformation($"Response: {result}");
                    exchangeRate = Convert.ToDouble(result);
                    exchangeRate = Math.Round(exchangeRate, 2);
                    return exchangeRate;
                }
                else
                {
                    _logger.LogError("Failed to fetch exchange rate from {0} to {1}. Status code: {2}", from, to, response.StatusCode);
                    return exchangeRate;
                }
            }
        }

    }
}