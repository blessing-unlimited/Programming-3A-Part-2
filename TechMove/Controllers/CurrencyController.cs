using Microsoft.AspNetCore.Mvc;
using TechMove.Services;

namespace TechMove.Controllers
{
    // Controller for providing asynchronous currency estimation to the UI
    public class CurrencyController : Controller
    {
        private readonly IExchangeRateServices _exchangeRateServices;

        // Constructor injects the external API service
        public CurrencyController(IExchangeRateServices exchangeRateServices)
        {
            _exchangeRateServices = exchangeRateServices;
        }

        // Currency/UsdToZarEstimate - Returns a JSON estimation for live UI updates
        [HttpGet]
        public async Task<IActionResult> UsdToZarEstimate(decimal usd, CancellationToken cancellationToken)
        {
            // Ensure the input amount is valid
            if (usd < 0)
                return BadRequest("USD amount must be zero or positive.");

            // Fetch the live rate and perform the calculation
            var rate = await _exchangeRateServices.GetUsdToZarRateAsync(cancellationToken);
            var zar = Math.Round(usd * rate, 2, MidpointRounding.AwayFromZero);
            var fetchedAtUtc = DateTime.UtcNow;

            //Return as JSON for consumption by JavaScript on the frontend
            return Json(new
            {
                usd,
                rate,
                zar,
                fetchedAtUtc
            });
        }
    }
}
