using AuthenticationApp.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace AuthenticationApp.Infrastructure.Persistence
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string adminEmail = "jan4ma@gmail.com";
            const string password = "JuanJad123!G";

            // 1. تأكد من أن الدور موجود
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            // 2. تحقق من وجود المستخدم
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin is null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "Juan Khanjar As System Administrator",
                    EmailConfirmed = true // أو false لو تريد إرساله
                };

                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                    await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }
}
