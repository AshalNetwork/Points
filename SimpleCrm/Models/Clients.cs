using SimpleCrm.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SimpleCrm.Models
{
    public class Clients
    {
        public Guid Id { get; set; }
        public string ClientName { get; set; }
        public string ClientPhone { get; set; }
        public string? Description { get; set; }
        public ClientStatusEnum Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public Guid? CommingReasonId { get; set; }
        public CommingReason? CommingReason { get; set; }
        public string? CustomReason { get; set; }
        public ICollection<FollowedClient> FollowedClients { get; set; }
    }
}
