namespace TechMove.Services
{
    public interface IExchangeRateServices
    {
        Task<decimal> GetUsdToZarRateAsync(CancellationToken cancellationToken = default);
    }
}
