using SimpleCrm.Abstraction;

namespace SimpleCrm.Models
{
    public class FollowedClient:BaseEntity
    {
        public Guid ClientId { get; set; }
        public Clients Client { get; set; }
        public string FromId { get; set; }
        public ApplicationUser From { get; set; }
        public string ToId { get; set; }
        public ApplicationUser To { get; set; }
    }
}
