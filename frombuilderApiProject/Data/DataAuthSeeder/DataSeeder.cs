// Data/DataSeeder.cs
using FormBuilder.API.Data;
using FormBuilder.API.Models;
using FormBuilder.API.Models.FormBuilder.API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

public class DataSeeder
{
    public static async Task SeedAsync(AuthDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        await context.Database.MigrateAsync();

        // إنشاء الأدوار الأساسية
        await SeedRoles(roleManager);

        // إنشاء المستخدمين
        await SeedUsers(userManager);

        // إنشاء الأدوار المخصصة والصلاحيات
        await SeedCustomRolesAndPermissions(context);

        // إنشاء بيانات تجريبية للفورم بيلدر
    }

    private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "Admin", "User", "Manager" };

        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
    }

    private static async Task SeedUsers(UserManager<AppUser> userManager)
    {
        // إنشاء مستخدم Admin
        var adminUser = new AppUser
        {
            UserName = "admin@formbuilder.com",
            Email = "admin@formbuilder.com",
            DisplayName = "System Administrator",
            IsActive = true,
            EmailConfirmed = true
        };

        if (await userManager.FindByEmailAsync(adminUser.Email) == null)
        {
            var result = await userManager.CreateAsync(adminUser, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        // إنشاء مستخدم عادي
        var normalUser = new AppUser
        {
            UserName = "user@formbuilder.com",
            Email = "user@formbuilder.com",
            DisplayName = "Regular User",
            IsActive = true,
            EmailConfirmed = true
        };

        if (await userManager.FindByEmailAsync(normalUser.Email) == null)
        {
            var result = await userManager.CreateAsync(normalUser, "User123!");
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, "User");
            }
        }
    }

    private static async Task SeedCustomRolesAndPermissions(AuthDbContext context)
    {
        if (!await context.CustomRoles.AnyAsync())
        {
            var roles = new List<Role>
            {
                new Role { RoleName = "FormCreator", Description = "Can create and manage forms", IsActive = true },
                new Role { RoleName = "FormViewer", Description = "Can view forms only", IsActive = true },
                new Role { RoleName = "DataEntry", Description = "Can fill form data", IsActive = true }
            };

            await context.CustomRoles.AddRangeAsync(roles);
            await context.SaveChangesAsync();
        }

        if (!await context.Permissions.AnyAsync())
        {
            var permissions = new List<Permission>
            {
                new Permission { PermissionName = "Forms.Create", Description = "Create new forms", Category = "Forms" },
                new Permission { PermissionName = "Forms.Edit", Description = "Edit existing forms", Category = "Forms" },
                new Permission { PermissionName = "Forms.Delete", Description = "Delete forms", Category = "Forms" },
                new Permission { PermissionName = "Forms.View", Description = "View forms", Category = "Forms" },
                new Permission { PermissionName = "Data.Create", Description = "Create form data", Category = "Data" },
                new Permission { PermissionName = "Data.View", Description = "View form data", Category = "Data" }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }
    }

}