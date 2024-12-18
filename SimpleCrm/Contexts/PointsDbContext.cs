using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SimpleCrm.Models;

namespace SimpleCrm.Contexts
{
    public class CrmDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public CrmDbContext(DbContextOptions<CrmDbContext> options)
           : base(options)
        {
        }
        public DbSet<Clients> Clients { get; set; }
        public DbSet<ValidationCode> ValidationCodes { get; set; }
        public DbSet<CommingReason> CommingReasons { get; set; }
        public DbSet<FollowedClient> FollowedClients { get; set; }
    }
}
