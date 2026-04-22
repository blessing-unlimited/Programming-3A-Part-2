using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TechMove.Models
{
    public class Contract
    {
        public int ContractId { get; set; }

        public int ClientId { get; set; }
        public Client? Client { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public String Status { get; set; }
        public String ServiceLevel { get; set; }


        public String SignedAgreementPath { get; set; }
        public String SignedAgreementFileName { get; set; }
        public DateOnly? AgreementUploadDate { get; set; }

        public ICollection<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
