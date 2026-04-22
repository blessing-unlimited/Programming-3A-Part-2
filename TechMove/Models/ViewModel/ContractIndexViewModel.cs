using TechMove.Models;

namespace TechMove.Models.ViewModel
{
    public class ContractIndexViewModel
    {
        public ContractFilterViewModel Filter { get; set; } = new();
        public IReadOnlyList<Contract> Contracts { get; set; } = Array.Empty<Contract>();
    }
}
