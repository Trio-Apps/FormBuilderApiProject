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
        await context.Database.EnsureCreatedAsync();

        // 1. Seed Roles
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

        // 2. Seed Permissions
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

        // 3. Seed SuperAdmin User
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

        var adminUser = await context.Users.FirstOrDefaultAsync(u => u.UserName == "admin");
        if (adminUser == null) throw new Exception("Admin user not found!");

        // 4. Seed Field Types - التصحيح هنا
        if (!await context.FIELD_TYPES.AnyAsync())
        {
            var fieldTypes = new[]
            {
                new FIELD_TYPES
                {
                    TypeName = "Text",
                    DataType = "string",
                    MaxLength = 200,
                    HasOptions = false,
                    AllowMultiple = false,
                    IsActive = true
                    // ❌ إزالة CreatedDate و CreatedByUserId لأنها غير موجودة في الـ Entity
                },
                new FIELD_TYPES
                {
                    TypeName = "Number",
                    DataType = "decimal",
                    MaxLength = null,
                    HasOptions = false,
                    AllowMultiple = false,
                    IsActive = true
                },
                new FIELD_TYPES
                {
                    TypeName = "Date",
                    DataType = "DateTime",
                    MaxLength = null,
                    HasOptions = false,
                    AllowMultiple = false,
                    IsActive = true
                },
                new FIELD_TYPES
                {
                    TypeName = "Dropdown",
                    DataType = "string",
                    MaxLength = null,
                    HasOptions = true,
                    AllowMultiple = false,
                    IsActive = true
                },
                new FIELD_TYPES
                {
                    TypeName = "Checkbox",
                    DataType = "bool",
                    MaxLength = null,
                    HasOptions = true,
                    AllowMultiple = true,
                    IsActive = true
                },
                new FIELD_TYPES
                {
                    TypeName = "Radio",
                    DataType = "string",
                    MaxLength = null,
                    HasOptions = true,
                    AllowMultiple = false,
                    IsActive = true
                },
                new FIELD_TYPES
                {
                    TypeName = "TextArea",
                    DataType = "string",
                    MaxLength = 1000,
                    HasOptions = false,
                    AllowMultiple = false,
                    IsActive = true
                },
                new FIELD_TYPES
                {
                    TypeName = "Email",
                    DataType = "string",
                    MaxLength = 100,
                    HasOptions = false,
                    AllowMultiple = false,
                    IsActive = true
                },
                new FIELD_TYPES
                {
                    TypeName = "Phone",
                    DataType = "string",
                    MaxLength = 20,
                    HasOptions = false,
                    AllowMultiple = false,
                    IsActive = true
                }
            };

            await context.FIELD_TYPES.AddRangeAsync(fieldTypes);
            await context.SaveChangesAsync();
        }

        // 5. Seed FORM_BUILDER أولاً
        if (!await context.FORM_BUILDER.AnyAsync())
        {
            var formBuilder = new FORM_BUILDER
            {
                FormName = "Sample Form",
                FormCode = "SAMPLE_FORM",
                Description = "A sample form for testing",
                Version = 1,
                IsPublished = true,
                IsActive = true,
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = adminUser.Id
            };

            await context.FORM_BUILDER.AddAsync(formBuilder);
            await context.SaveChangesAsync();
        }

        // 6. Seed FORM_TABS
        if (!await context.FORM_TABS.AnyAsync())
        {
            var formBuilder = await context.FORM_BUILDER.FirstOrDefaultAsync();
            if (formBuilder == null) throw new Exception("No FORM_BUILDER found!");

            var formTabs = new[]
            {
                new FORM_TABS
                {
                    FormBuilderId = formBuilder.id,
                    TabName = "Personal Information",
                    TabCode = "PERSONAL_INFO",
                    TabOrder = 1,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = adminUser.Id
                },
                new FORM_TABS
                {
                    FormBuilderId = formBuilder.id,
                    TabName = "Contact Information",
                    TabCode = "CONTACT_INFO",
                    TabOrder = 2,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                    CreatedByUserId = adminUser.Id
                }
            };

            await context.FORM_TABS.AddRangeAsync(formTabs);
            await context.SaveChangesAsync();
        }

        //// 7. Seed FORM_FIELDS
        //if (!await context.FORM_FIELDS.AnyAsync())
        //{
        //    var formTab = await context.FORM_TABS.FirstOrDefaultAsync();
        //    if (formTab == null) throw new Exception("No FORM_TAB found!");

        //    var textFieldType = await context.FIELD_TYPES.FirstOrDefaultAsync(ft => ft.TypeName == "Text");
        //    if (textFieldType == null) throw new Exception("Text FieldType not found!");

        //    var numberFieldType = await context.FIELD_TYPES.FirstOrDefaultAsync(ft => ft.TypeName == "Number");
        //    var emailFieldType = await context.FIELD_TYPES.FirstOrDefaultAsync(ft => ft.TypeName == "Email");
        //    var phoneFieldType = await context.FIELD_TYPES.FirstOrDefaultAsync(ft => ft.TypeName == "Phone");
        //    var dropdownFieldType = await context.FIELD_TYPES.FirstOrDefaultAsync(ft => ft.TypeName == "Dropdown");

        //    var formFields = new[]
        //    {
        //        // Personal Information Tab
        //        new FORM_FIELDS
        //        {
        //            TabId = formTab.id,
        //            FieldTypeId = 5,
        //            FieldName = "First Name",
        //            FieldCode = "FIRST_NAME",
        //            FieldOrder = 1,
        //            Placeholder = "Enter your first name",
        //            HintText = "Please enter your legal first name",
        //            IsMandatory = true,
        //            IsEditable = true,
        //            IsVisible = true,
        //            DataType = "string",
        //            MaxLength = 50,
        //            IsActive = true,
        //            CreatedDate = DateTime.UtcNow,
        //            CreatedByUserId = adminUser.Id
        //        },
        //        new FORM_FIELDS
        //        {
        //            TabId = formTab.id,
        //            FieldTypeId = 5,
        //            FieldName = "Last Name",
        //            FieldCode = "LAST_NAME",
        //            FieldOrder = 2,
        //            Placeholder = "Enter your last name",
        //            HintText = "Please enter your legal last name",
        //            IsMandatory = true,
        //            IsEditable = true,
        //            IsVisible = true,
        //            DataType = "string",
        //            MaxLength = 50,
        //            IsActive = true,
        //            CreatedDate = DateTime.UtcNow,
        //            CreatedByUserId = adminUser.Id
        //        },
        //        new FORM_FIELDS
        //        {
        //            TabId = formTab.id,
        //            FieldTypeId = 5,
        //            FieldName = "Age",
        //            FieldCode = "AGE",
        //            FieldOrder = 3,
        //            Placeholder = "Enter your age",
        //            HintText = "Must be between 18 and 100",
        //            IsMandatory = false,
        //            IsEditable = true,
        //            IsVisible = true,
        //            DataType = "int",
        //            MinValue = 18,
        //            MaxValue = 100,
        //            IsActive = true,
        //            CreatedDate = DateTime.UtcNow,
        //            CreatedByUserId = adminUser.Id
        //        },
        //        new FORM_FIELDS
        //        {
        //            TabId = formTab.id,
        //            FieldTypeId = 5,
        //            FieldName = "Gender",
        //            FieldCode = "GENDER",
        //            FieldOrder = 4,
        //            Placeholder = "Select your gender",
        //            HintText = "Choose from the options",
        //            IsMandatory = false,
        //            IsEditable = true,
        //            IsVisible = true,
        //            DataType = "string",
        //            IsActive = true,
        //            CreatedDate = DateTime.UtcNow,
        //            CreatedByUserId = adminUser.Id
        //        }
        //    };

        //    await context.FORM_FIELDS.AddRangeAsync(formFields);
        //    await context.SaveChangesAsync();
        //}

        // 8. Seed FIELD_OPTIONS لحقل Gender
        if (!await context.FIELD_OPTIONS.AnyAsync())
        {
            var genderField = await context.FORM_FIELDS.FirstOrDefaultAsync(f => f.FieldCode == "GENDER");
            if (genderField != null)
            {
                var fieldOptions = new[]
                {
                    new FIELD_OPTIONS
                    {
                        FieldId = genderField.id,
                        OptionText = "Male",
                        OptionValue = "M",
                        OptionOrder = 1,
                        IsDefault = false,
                        IsActive = true
                    },
                    new FIELD_OPTIONS
                    {
                        FieldId = genderField.id,
                        OptionText = "Female",
                        OptionValue = "F",
                        OptionOrder = 2,
                        IsDefault = false,
                        IsActive = true
                    },
                    new FIELD_OPTIONS
                    {
                        FieldId = genderField.id,
                        OptionText = "Other",
                        OptionValue = "O",
                        OptionOrder = 3,
                        IsDefault = true,
                        IsActive = true
                    }
                };

                await context.FIELD_OPTIONS.AddRangeAsync(fieldOptions);
                await context.SaveChangesAsync();
            }
        }

        Console.WriteLine("✅ Database seeding completed successfully!");
        Console.WriteLine($"   - Roles: {await roleManager.Roles.CountAsync()}");
        Console.WriteLine($"   - Users: {await userManager.Users.CountAsync()}");
        Console.WriteLine($"   - Permissions: {await context.Permissions.CountAsync()}");
        Console.WriteLine($"   - Field Types: {await context.FIELD_TYPES.CountAsync()}");
        Console.WriteLine($"   - Form Builders: {await context.FORM_BUILDER.CountAsync()}");
        Console.WriteLine($"   - Form Tabs: {await context.FORM_TABS.CountAsync()}");
        Console.WriteLine($"   - Form Fields: {await context.FORM_FIELDS.CountAsync()}");
        Console.WriteLine($"   - Field Options: {await context.FIELD_OPTIONS.CountAsync()}");
    }
}