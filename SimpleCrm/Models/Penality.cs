namespace SimpleCrm.Models
{
    public class Penality
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public string Reason { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
