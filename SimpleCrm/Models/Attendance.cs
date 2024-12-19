namespace SimpleCrm.Models
{
    public class Attendance
    {
        public Guid Id { get; set; }
        public TimeSpan CheckIn { get; set; }
        public TimeSpan CheckOut { get; set; }
        public DateTime Date { get; set; }

        public string ApplicationUserId { get; set; } = null!;
        public ApplicationUser ApplicationUser { get; set; } = null!;

    }
}
