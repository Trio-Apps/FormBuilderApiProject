// DataSeeder.cs مبسط
using FormBuilder.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.API.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AuthDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("🔧 Starting database seeding...");

            // تطبيق الـ Migrations
            await context.Database.MigrateAsync();

            // 1. إنشاء الأدوار
            await SeedRolesAsync(roleManager);

            // 2. إنشاء المستخدمين
            await SeedUsersAsync(userManager);

            Console.WriteLine("🎉 Database seeding completed successfully!");
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "User", "Manager" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine($"✅ Created role: {role}");
                }
            }
        }

        private static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            var adminUser = await userManager.FindByEmailAsync("admin@formbuilder.com");

            if (adminUser == null)
            {
                var user = new AppUser
                {
                    UserName = "admin@formbuilder.com",
                    Email = "admin@formbuilder.com",
                    DisplayName = "System Administrator",
                    IsActive = true,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "Admin");
                    Console.WriteLine("✅ Created admin user: admin@formbuilder.com");
                }
            }
        }
    }
}