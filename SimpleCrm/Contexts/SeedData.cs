using Microsoft.AspNetCore.Identity;
using SimpleCrm.Models;

namespace SimpleCrm.Contexts
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            var adminRole = "User";

            // Ensure the admin role exists
            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }
            // userName MohamedMarey11@gmail.com  pass Marey#159
            // userName amasser@myfatoorah.com  pass Amasser#185
            // userName mostafa102@gmail.com pass Mostafa#105
            // Seed a user
            var defaultUser = new ApplicationUser { UserName = "mostafa102@gmail.com", Email = "mostafa102@gmail.com", EmailConfirmed = true };

            if (userManager.Users.All(u => u.UserName != defaultUser.UserName))
            {
                var result = await userManager.CreateAsync(defaultUser, "Mostafa#105");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(defaultUser, adminRole);
                }
            }
        }
    }
}
