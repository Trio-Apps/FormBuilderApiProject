// DataSeeder.cs
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

            // تأكد من أن Database موجودة
            await context.Database.EnsureCreatedAsync();

            // 1. إنشاء الأدوار إذا لم تكن موجودة
            string[] roles = { "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(new IdentityRole(role));
                    Console.WriteLine(result.Succeeded ?
                        $"✅ Created role: {role}" :
                        $"❌ Failed to create role: {role} - {string.Join(", ", result.Errors)}");
                }
                else
                {
                    Console.WriteLine($"✅ Role already exists: {role}");
                }
            }

            // 2. إنشاء مستخدم Admin إذا لم يكن موجوداً
            var adminEmail = "admin@example.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new AppUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    DisplayName = "System Administrator"
                };

                var createResult = await userManager.CreateAsync(adminUser, "Admin123!");
                if (createResult.Succeeded)
                {
                    Console.WriteLine($"✅ Created admin user: {adminEmail}");
                }
                else
                {
                    Console.WriteLine($"❌ Admin creation failed: {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                    return; // توقف إذا فشل إنشاء المستخدم
                }
            }
            else
            {
                Console.WriteLine($"✅ Admin user already exists: {adminEmail}");
            }

            // 3. ربط المستخدم بدور Admin (هذه هي الخطوة المهمة!)
            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                var addToRoleResult = await userManager.AddToRoleAsync(adminUser, "Admin");
                if (addToRoleResult.Succeeded)
                {
                    Console.WriteLine($"✅ Successfully added user '{adminEmail}' to 'Admin' role");
                }
                else
                {
                    Console.WriteLine($"❌ Failed to add user to Admin role: {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                }
            }
            else
            {
                Console.WriteLine($"✅ User '{adminEmail}' is already in 'Admin' role");
            }

            // 4. إنشاء مستخدم عادي للاختبار
            var userEmail = "user@example.com";
            var normalUser = await userManager.FindByEmailAsync(userEmail);

            if (normalUser == null)
            {
                normalUser = new AppUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true,
                    DisplayName = "Regular User"
                };

                var createResult = await userManager.CreateAsync(normalUser, "User123!");
                if (createResult.Succeeded)
                {
                    await userManager.AddToRoleAsync(normalUser, "User");
                    Console.WriteLine($"✅ Created normal user: {userEmail}");
                }
            }
            else
            {
                Console.WriteLine($"✅ Normal user already exists: {userEmail}");
            }

            Console.WriteLine("🎉 Database seeding completed successfully!");
        }
    }
}