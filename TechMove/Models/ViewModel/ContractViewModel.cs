using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace TechMove.Models.ViewModel
{
    public class ContractViewModel
    {

        public int ClientId { get; set; }
        public int ContractId { get; set; }

        public DateOnly StartDate { get; set; }

        public DateOnly EndDate { get; set; }


        public static readonly String[] AllowedStatuses = ["Draft", "Active", "Expired", "On Hold"];
        public String Status { get; set; } = "Draft";
        public String ServiceLevel { get; set; }
        public IFormFile? SignedAgreementFile { get; set; }

        public String? CurrentFileName { get; set; }

    }
}
