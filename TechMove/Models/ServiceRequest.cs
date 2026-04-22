namespace TechMove.Models
{
    public class ServiceRequest
    {
        public int ServiceRequestId { get; set; }

        public int ContractId { get; set; }
        public Contract? Contract { get; set; }
        public String Description { get; set; }
        
        public decimal CostUsd { get; set; }
        public decimal ExchangeRateUsdToZar { get; set; }
        public decimal CostZar { get; set; }
        public DateTime ExchangeRateFetchedAtUtc { get; set; }
        
        public String Status { get; set; }
    }
}
