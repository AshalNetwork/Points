using Microsoft.AspNetCore.Identity;
using SimpleCrm.Enums;
using SimpleCrm.Models;

namespace SimpleCrm.Contexts
{
    public static class SeedData
    {
        public static async Task InitializeUsersAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var Role = RolesEnum.ProductionMangerB.ToString();

            // Ensure the admin role exists
            if (!await roleManager.RoleExistsAsync(Role))
            {
                await roleManager.CreateAsync(new IdentityRole(Role));
            }

            // Seed a user
            var defaultUser = new ApplicationUser { Name = "Hossam Elganiny", UserName = "hossamelsayed676@gmail.com", Email = "hossamelsayed676@gmail.com", EmailConfirmed = true };

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                var result = await userManager.CreateAsync(defaultUser, "Hossam#105");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, Role);
                }
            }
        }
    }
}
