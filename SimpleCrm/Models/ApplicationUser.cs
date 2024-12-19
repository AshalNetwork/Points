using Microsoft.AspNetCore.Identity;

namespace SimpleCrm.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        public ICollection<Tasks> Tasks { get; set; } = new HashSet<Tasks>();

    }
}
