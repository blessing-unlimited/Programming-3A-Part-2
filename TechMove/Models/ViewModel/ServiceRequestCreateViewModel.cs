using System.ComponentModel.DataAnnotations;

namespace TechMove.Models.ViewModel
{
    public class ServiceRequestCreateViewModel
    {
        public int ContractId { get; set; }
        public int ServiceRequestId { get; set; }
        public String  Description { get; set; }
        public decimal CostUsd { get; set; }
        public String Status { get; set; }
    }
}
