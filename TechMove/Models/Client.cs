namespace TechMove.Models
{
    public class Client
    {
        public int ClientId { get; set; }
        public String ClientName { get; set; }
        public String ContactDetails { get; set; }
        public String Region { get; set; }

        public ICollection<Contract> Contracts { get; set; } = new List<Contract>();
    }
}
