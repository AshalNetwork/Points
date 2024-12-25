using SimpleCrm.Enums;

namespace SimpleCrm.Models
{
    public class Tasks
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public StatusEnums Status { get; set; }
        public DateTime StartAt { get; set; }
        public DateTime EndAt { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public string? CompletedBy { get; set; }
    }
}
