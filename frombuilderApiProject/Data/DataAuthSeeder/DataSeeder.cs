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
            await SeedRolesAsync(roleManager);

            // 2. إنشاء المستخدمين وإضافة الأدوار لهم
            await SeedUsersAsync(userManager);

            // 3. إنشاء الصلاحيات
            await SeedPermissionsAsync(context);

            // 4. ربط الأدوار بالصلاحيات
            await SeedRolePermissionsAsync(context, roleManager);

            Console.WriteLine("🎉 Database seeding completed successfully!");
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("📋 Seeding roles...");

            string[] roles = { "Admin", "User", "Manager" };

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
        }

        private static async Task SeedUsersAsync(UserManager<AppUser> userManager)
        {
            Console.WriteLine("👥 Seeding users...");

            var users = new[]
            {
                new { Email = "admin@example.com", Password = "Admin123!", DisplayName = "System Administrator", Roles = new[] { "Admin" } },
                new { Email = "manager@example.com", Password = "Manager123!", DisplayName = "Project Manager", Roles = new[] { "Manager" } },
                new { Email = "user1@example.com", Password = "User123!", DisplayName = "Regular User 1", Roles = new[] { "User" } },
                new { Email = "user2@example.com", Password = "User123!", DisplayName = "Regular User 2", Roles = new[] { "User" } }
            };

            foreach (var userInfo in users)
            {
                var user = await userManager.FindByEmailAsync(userInfo.Email);

                if (user == null)
                {
                    user = new AppUser
                    {
                        UserName = userInfo.Email,
                        Email = userInfo.Email,
                        EmailConfirmed = true,
                        DisplayName = userInfo.DisplayName,
                        IsActive = true,
                        CreatedDate = DateTime.UtcNow
                    };

                    var createResult = await userManager.CreateAsync(user, userInfo.Password);
                    if (createResult.Succeeded)
                    {
                        Console.WriteLine($"✅ Created user: {userInfo.Email}");
                    }
                    else
                    {
                        Console.WriteLine($"❌ User creation failed: {userInfo.Email} - {string.Join(", ", createResult.Errors.Select(e => e.Description))}");
                        continue;
                    }
                }
                else
                {
                    Console.WriteLine($"✅ User already exists: {userInfo.Email}");
                }

                // إضافة الأدوار للمستخدم
                foreach (var role in userInfo.Roles)
                {
                    if (!await userManager.IsInRoleAsync(user, role))
                    {
                        var addToRoleResult = await userManager.AddToRoleAsync(user, role);
                        if (addToRoleResult.Succeeded)
                        {
                            Console.WriteLine($"✅ Added user '{userInfo.Email}' to role '{role}'");
                        }
                        else
                        {
                            Console.WriteLine($"❌ Failed to add user to role: {userInfo.Email} -> {role} - {string.Join(", ", addToRoleResult.Errors.Select(e => e.Description))}");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"✅ User '{userInfo.Email}' is already in role '{role}'");
                    }
                }
            }
        }

        private static async Task SeedPermissionsAsync(AuthDbContext context)
        {
            Console.WriteLine("🔐 Seeding permissions...");

            var permissions = new[]
            {
                new Permission { PermissionName = "Users.View", Description = "View users list" },
                new Permission { PermissionName = "Users.Create", Description = "Create new users" },
                new Permission { PermissionName = "Users.Edit", Description = "Edit existing users" },
                new Permission { PermissionName = "Users.Delete", Description = "Delete users" },
                new Permission { PermissionName = "Roles.View", Description = "View roles list" },
                new Permission { PermissionName = "Roles.Create", Description = "Create new roles" },
                new Permission { PermissionName = "Roles.Edit", Description = "Edit existing roles" },
                new Permission { PermissionName = "Roles.Delete", Description = "Delete roles" },
                new Permission { PermissionName = "Permissions.View", Description = "View permissions" },
                new Permission { PermissionName = "Permissions.Assign", Description = "Assign permissions to roles" },
                new Permission { PermissionName = "Forms.View", Description = "View forms" },
                new Permission { PermissionName = "Forms.Create", Description = "Create forms" },
                new Permission { PermissionName = "Forms.Edit", Description = "Edit forms" },
                new Permission { PermissionName = "Forms.Delete", Description = "Delete forms" },
                new Permission { PermissionName = "Reports.View", Description = "View reports" },
                new Permission { PermissionName = "Reports.Generate", Description = "Generate reports" }
            };

            foreach (var permission in permissions)
            {
                var existingPermission = await context.Permissions
                    .FirstOrDefaultAsync(p => p.PermissionName == permission.PermissionName);

                if (existingPermission == null)
                {
                    permission.CreatedDate = DateTime.UtcNow;
                    context.Permissions.Add(permission);
                    Console.WriteLine($"✅ Created permission: {permission.PermissionName}");
                }
                else
                {
                    Console.WriteLine($"✅ Permission already exists: {permission.PermissionName}");
                }
            }

            await context.SaveChangesAsync();
        }

        private static async Task SeedRolePermissionsAsync(AuthDbContext context, RoleManager<IdentityRole> roleManager)
        {
            Console.WriteLine("🔗 Seeding role permissions...");

            // الحصول على جميع الأدوار والصلاحيات
            var roles = await roleManager.Roles.ToListAsync();
            var permissions = await context.Permissions.ToListAsync();

            var rolePermissions = new List<RolePermission>();

            foreach (var role in roles)
            {
                var permissionsToAdd = GetPermissionsForRole(role.Name, permissions);

                foreach (var permission in permissionsToAdd)
                {
                    var existingRolePermission = await context.RolePermissions
                        .FirstOrDefaultAsync(rp => rp.RoleID == role.Id && rp.PermissionID == permission.PermissionID);

                    if (existingRolePermission == null)
                    {
                        var rolePermission = new RolePermission
                        {
                            RoleID = role.Id,
                            PermissionID = permission.PermissionID,
                            AssignedDate = DateTime.UtcNow
                        };
                        rolePermissions.Add(rolePermission);
                        Console.WriteLine($"✅ Added permission '{permission.PermissionName}' to role '{role.Name}'");
                    }
                    else
                    {
                        Console.WriteLine($"✅ Role permission already exists: {role.Name} -> {permission.PermissionName}");
                    }
                }
            }

            if (rolePermissions.Any())
            {
                await context.RolePermissions.AddRangeAsync(rolePermissions);
                await context.SaveChangesAsync();
            }
        }

        private static List<Permission> GetPermissionsForRole(string roleName, List<Permission> allPermissions)
        {
            return roleName switch
            {
                "Admin" => allPermissions, // Admin لديه جميع الصلاحيات
                "Manager" => allPermissions.Where(p =>
                    p.PermissionName.StartsWith("Forms.") ||
                    p.PermissionName.StartsWith("Reports.") ||
                    p.PermissionName == "Users.View").ToList(),
                "User" => allPermissions.Where(p =>
                    p.PermissionName == "Forms.View" ||
                    p.PermissionName == "Forms.Create" ||
                    p.PermissionName == "Reports.View").ToList(),
                _ => new List<Permission>()
            };
        }
    }
}