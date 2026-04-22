using System.ComponentModel.DataAnnotations;

namespace TechMove.Models.ViewModel
{
    public class ContractFilterViewModel
    {
        public DateOnly? FromDate { get; set; }
        public DateOnly? ToDate { get; set; }
        public string? Status { get; set; }

        public static readonly string[] AllowedStatuses = ContractViewModel.AllowedStatuses;
    }
}
