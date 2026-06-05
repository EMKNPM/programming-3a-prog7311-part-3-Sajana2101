namespace GLMS2.Interfaces
{
    // Handles currency conversion logic for service request costs
    // Allows USD values to be converted to ZAR using an external exchange rate
    public interface ICurrencyService
    {
        // Retrieves the latest USD to ZAR exchange rate
        Task<decimal> GetUsdToZarRateAsync();

        // Converts USD amount to ZAR using the exchange rate
        decimal ConvertUsdToZar(decimal usdAmount, decimal exchangeRate);
    }
}