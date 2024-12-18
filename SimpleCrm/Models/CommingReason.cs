using SimpleCrm.Abstraction;

namespace SimpleCrm.Models
{
    public class CommingReason:BaseEntity
    {
        public string Reason { get; set; }
        public ICollection<Clients> Clients { get; set; } = new HashSet<Clients>();
    }
}
