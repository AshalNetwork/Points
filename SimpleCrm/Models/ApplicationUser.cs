using Microsoft.AspNetCore.Identity;

namespace SimpleCrm.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public string? DeviceId { get; set; }
        public ICollection<Tasks> Tasks { get; set; } = new HashSet<Tasks>();
        public ICollection<Attendance> Attendances { get; set; } = new HashSet<Attendance>();
        public ICollection<UserPoint> UserPoints { get; set; } = new HashSet<UserPoint>();
        public ICollection<Report> Reports { get; set; } = new HashSet<Report>();
        public ICollection<Penality> Penalities { get; set; } = new HashSet<Penality>();
        public ICollection<Sale> Sales { get; set; } = new HashSet<Sale>();

    }
}
