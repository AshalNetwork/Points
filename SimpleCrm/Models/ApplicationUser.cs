using Microsoft.AspNetCore.Identity;

namespace SimpleCrm.Models
{
    public class ApplicationUser: IdentityUser
    {
        public string Name { get; set; }
        //public ICollection<Clients> Clients { get; set; } = new HashSet<Clients>();

    }
}
