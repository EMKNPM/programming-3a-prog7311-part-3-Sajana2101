using GLMS2.Interfaces;
using GLMS2.Models;
using System.Text.Json;

namespace GLMS2.Services
{
    // Handles currency conversion using an external exchange rate API
    public class CurrencyService : ICurrencyService
    {
        private readonly HttpClient _httpClient;

        public CurrencyService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        // Retrieves the latest USD to ZAR exchange rate from the API
        public async Task<decimal> GetUsdToZarRateAsync()
        {
            try
            {
                var url = "https://open.er-api.com/v6/latest/USD";

                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                // Converts API response from JSON into a C# object
                var json = await response.Content.ReadAsStringAsync();

                var apiResponse = JsonSerializer.Deserialize<CurrencyApiResponse>(json);
                // Ensures ZAR rate exists in the response
                if (apiResponse?.Rates == null || !apiResponse.Rates.ContainsKey("ZAR"))
                {
                    // Handles API errors gracefully
                    throw new InvalidOperationException("ZAR exchange rate not found.");
                }

                return apiResponse.Rates["ZAR"];
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Unable to retrieve exchange rate.", ex);
            }
        }
        // Converts USD value to ZAR using the exchange rate
        public decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate)
        {
            // Prevents invalid currency values
            if (usdAmount < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(usdAmount), "USD amount cannot be negative.");
            }

            if (exchangeRate <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exchangeRate), "Exchange rate must be greater than zero.");
            }
            // Rounds result to 2 decimal places for currency formatting
            return Math.Round(usdAmount * exchangeRate, 2);
        }
    }
}