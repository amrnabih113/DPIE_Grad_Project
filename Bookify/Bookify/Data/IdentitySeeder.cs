
namespace Bookify.Data
{
    public class IdentitySeeder
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roleNames = { "Admin", "User" };
            foreach(var rolename in roleNames)
            {
                var roleExists = await roleManager.RoleExistsAsync(rolename);
                if (!roleExists)
                    await roleManager.CreateAsync(new IdentityRole(rolename));
            }
        }
        public static async Task SeedAdminAccount(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var adminEmail = "admin@bookify.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if(adminUser == null)
            {
                var user = new ApplicationUser
                {
                    FullName = adminEmail,
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed =true

                };
                var result = await userManager.CreateAsync(user,"Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                }
            }
        }
    }
}
