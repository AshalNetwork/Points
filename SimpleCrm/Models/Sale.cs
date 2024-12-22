using SimpleCrm.Enums;

namespace SimpleCrm.Models
{
    public class Sale
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public SalesRequestEnum Status { get; set; } 
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
