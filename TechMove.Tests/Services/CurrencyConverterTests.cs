using TechMove.Services;
using Xunit;

namespace TechMove.Tests.Services
{
    public class CurrencyConverterTests
    {
        [Theory]
        [InlineData(10, 18.5, 185.00)]   // 10 USD * 18.5 rate = 185 ZAR
        [InlineData(10.55, 18.25, 192.54)] // Testing rounding (192.5375 rounds to 192.54)
        [InlineData(0, 18.5, 0.00)]      // Zero value check
        public void ConvertToZar_ShouldCalculateCorrectAmount(decimal usd, decimal rate, decimal expected)
        {
            // Arrange (Set up the tool)
            var converter = new CurrencyConverter();

            // Act (Run the tool)
            var result = converter.ConvertToZar(usd, rate);

            // Assert (Check if it's correct)
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ConvertToZar_NegativeUsd_ShouldThrowError()
        {
            var converter = new CurrencyConverter();
            Assert.Throws<ArgumentException>(() => converter.ConvertToZar(-1, 18.5m));
        }
    }
}