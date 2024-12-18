using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleCrm.Models;

namespace SimpleCrm.Contexts
{
    public class PointsDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public PointsDbContext(DbContextOptions<PointsDbContext> options)
           : base(options)
        {
        }
        public DbSet<ValidationCode> ValidationCodes { get; set; }
        
    }
}
