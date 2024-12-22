namespace SimpleCrm.Models
{
    public class Report
    {
        public Guid Id { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
