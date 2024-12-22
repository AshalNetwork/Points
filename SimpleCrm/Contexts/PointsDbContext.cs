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
        public DbSet<PointsValue> PointsValues { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        public DbSet<UserPoint> UserPoints { get; set; }
        public DbSet<Penality> Penalities { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Sale> Sales { get; set; }
        
    }
}
