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
    public static async Task SeedAsync(FormBuilderDbContext context)
    {
        await context.Database.EnsureCreatedAsync();

        

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
                    FormBuilderId = formBuilder.Id,
                    TabName = "Personal Information",
                    TabCode = "PERSONAL_INFO",
                    TabOrder = 1,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
                },
                new FORM_TABS
                {
                    FormBuilderId = formBuilder.Id,
                    TabName = "Contact Information",
                    TabCode = "CONTACT_INFO",
                    TabOrder = 2,
                    IsActive = true,
                    CreatedDate = DateTime.UtcNow,
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
                        FieldId = genderField.Id,
                        OptionText = "Male",
                        OptionValue = "M",
                        OptionOrder = 1,
                        IsDefault = false,
                        IsActive = true
                    },
                    new FIELD_OPTIONS
                    {
                        FieldId = genderField.Id,
                        OptionText = "Female",
                        OptionValue = "F",
                        OptionOrder = 2,
                        IsDefault = false,
                        IsActive = true
                    },
                    new FIELD_OPTIONS
                    {
                        FieldId = genderField.Id,
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
       
        Console.WriteLine($"   - Field Types: {await context.FIELD_TYPES.CountAsync()}");
        Console.WriteLine($"   - Form Builders: {await context.FORM_BUILDER.CountAsync()}");
        Console.WriteLine($"   - Form Tabs: {await context.FORM_TABS.CountAsync()}");
        Console.WriteLine($"   - Form Fields: {await context.FORM_FIELDS.CountAsync()}");
        Console.WriteLine($"   - Field Options: {await context.FIELD_OPTIONS.CountAsync()}");
    }
}