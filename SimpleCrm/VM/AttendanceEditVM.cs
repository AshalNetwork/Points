using System.ComponentModel.DataAnnotations;

namespace SimpleCrm.VM
{
    public class AttendanceEditVM
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        public string ApplicationUserId { get; set; } = null!;

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public TimeSpan CheckIn { get; set; }

        [Required]
        public TimeSpan CheckOut { get; set; }
    }
}

