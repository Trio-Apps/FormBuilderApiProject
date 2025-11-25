using FormBuilder.API.Data;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Entitys.froms;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

public static class DataSeeder
{
    public static async Task SeedAsync(FormBuilderDbContext context, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        // تأكد من إنشاء قاعدة البيانات
        await context.Database.EnsureCreatedAsync();

        // ========================
        // 1. Seed Roles
        // ========================
        if (!await roleManager.Roles.AnyAsync())
        {
            var roles = new[]
            {
                new IdentityRole { Name = "SuperAdmin", NormalizedName = "SUPERADMIN" },
                new IdentityRole { Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Name = "User", NormalizedName = "USER" },
                new IdentityRole { Name = "Manager", NormalizedName = "MANAGER" },
                new IdentityRole { Name = "Approver", NormalizedName = "APPROVER" }
            };

            foreach (var role in roles)
            {
                await roleManager.CreateAsync(role);
            }
        }

        // ========================
        // 2. Seed Permissions
        // ========================
        if (!await context.Permissions.AnyAsync())
        {
            var permissions = new[]
            {
                new Permission { PermissionName = "Forms.Create", Description = "Create forms" },
                new Permission { PermissionName = "Forms.View", Description = "View forms" },
                new Permission { PermissionName = "Forms.Edit", Description = "Edit forms" },
                new Permission { PermissionName = "Forms.Delete", Description = "Delete forms" },
                new Permission { PermissionName = "Submissions.Create", Description = "Create submissions" },
                new Permission { PermissionName = "Submissions.View", Description = "View submissions" },
                new Permission { PermissionName = "Submissions.Approve", Description = "Approve submissions" },
                new Permission { PermissionName = "Submissions.Delete", Description = "Delete submissions" },
                new Permission { PermissionName = "Users.Manage", Description = "Manage users" },
                new Permission { PermissionName = "Roles.Manage", Description = "Manage roles" },
                new Permission { PermissionName = "Settings.Manage", Description = "Manage settings" }
            };

            await context.Permissions.AddRangeAsync(permissions);
            await context.SaveChangesAsync();
        }

        // ========================
        // 3. Seed SuperAdmin User
        // ========================
        if (!await userManager.Users.AnyAsync(u => u.UserName == "admin"))
        {
            var superAdmin = new AppUser
            {
                UserName = "admin",
                Email = "admin@zmbuildstr.com",
                EmailConfirmed = true,
                DisplayName = "System Administrator",
                PhoneNumber = "01234567890",
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(superAdmin, "Admin123!");
            if (result.Succeeded)
            {
                await userManager.AddToRolesAsync(superAdmin, new[] { "SuperAdmin", "Admin", "User" });
            }
        }

        // ========================
    

        // ========================
        // 5. Seed ALERT_RULES
        // ========================
        if (!await context.ALERT_RULES.AnyAsync())
        {
            var emailTemplate = await context.EMAIL_TEMPLATES.FirstOrDefaultAsync();
            if (emailTemplate != null)
            {
                var alertRules = new[]
                {
                    new ALERT_RULES
                    {
                        RuleName = "New Submission Alert",
                        TriggerType = "OnSubmissionCreate",
                        EmailTemplateId = emailTemplate.Id,
                        IsActive = true
                    }
                };
                await context.ALERT_RULES.AddRangeAsync(alertRules);
                await context.SaveChangesAsync();
            }
        }

        // ========================
        // 6. Optional: Log Summary
        // ========================
        Console.WriteLine("✅ Database seeding completed successfully!");
        Console.WriteLine("📊 Seeded Data Summary:");
        Console.WriteLine($"   - Roles: {await roleManager.Roles.CountAsync()}");
        Console.WriteLine($"   - Users: {await userManager.Users.CountAsync()}");
        Console.WriteLine($"   - Permissions: {await context.Permissions.CountAsync()}");
        Console.WriteLine($"   - Role Permissions: {await context.RolePermissions.CountAsync()}");
        Console.WriteLine($"   - Field Types: {await context.FIELD_TYPES.CountAsync()}");
        Console.WriteLine($"   - Document Types: {await context.DOCUMENT_TYPES.CountAsync()}");
        Console.WriteLine($"   - Projects: {await context.PROJECTS.CountAsync()}");
        Console.WriteLine($"   - Form Builders: {await context.FORM_BUILDER.CountAsync()}");
        Console.WriteLine($"   - Approval Workflows: {await context.APPROVAL_WORKFLOWS.CountAsync()}");
        Console.WriteLine($"   - Email Templates: {await context.EMAIL_TEMPLATES.CountAsync()}");
        Console.WriteLine($"   - Attachment Types: {await context.ATTACHMENT_TYPES.CountAsync()}");
    }
}
