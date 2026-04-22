namespace TechMove.Services
{
    // Interface defines the 'contract' for what a currency converter must do
    public interface ICurrencyConverter
    {
        decimal ConvertToZar(decimal usdAmount, decimal exchangeRate);
    }

    // Implementation of the currency conversion logic
    public class CurrencyConverter : ICurrencyConverter
    {
        // Converts USD to ZAR and applies financial rounding rules
        public decimal ConvertToZar(decimal usdAmount, decimal exchangeRate)
        {
            //  We cannot process negative money
            if (usdAmount < 0) throw new ArgumentException("USD amount cannot be negative.");

            //Multiply amount by rate and round to 2 decimal places (cents)
            // AwayFromZero ensures 0.005 rounds up to 0.01
            return Math.Round(usdAmount * exchangeRate, 2, MidpointRounding.AwayFromZero);
        }
    }
}