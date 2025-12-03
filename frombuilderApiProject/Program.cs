using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Data;
using FormBuilder.API.Models;
using FormBuilder.core.Repository;
using FormBuilder.Core.IServices.Auth;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Core.IServices.FormBuilder.FormBuilder.Services.Services;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Interfaces;
using FormBuilder.Infrastructure.Repositories;
using FormBuilder.Infrastructure.Repository;
using FormBuilder.Services;
using FormBuilder.Services.Repository;
using FormBuilder.Services.Services;
using FormBuilder.Services.Services.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --------------------------------------------------
// Controllers + JSON
// --------------------------------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();

// --------------------------------------------------
// Swagger Configuration - FIXED
// --------------------------------------------------
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FormBuilder API",
        Version = "v1",
        Description = "FormBuilder API Documentation"
    });

    // JWT Security Definition
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter the Bearer token like: Bearer {your token}",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = JwtBearerDefaults.AuthenticationScheme
                }
            },
            Array.Empty<string>()
        }
    });

    // Õ· „‘«ﬂ· «·‹ Schema
    options.UseAllOfToExtendReferenceSchemas();
    options.UseAllOfForInheritance();
    options.UseOneOfForPolymorphism();

    // Õ· „‘ﬂ·… «·‹ Schema IDs
    options.CustomSchemaIds(type => type.FullName?.Replace("+", "."));

    // ≈÷«›… XML Documentation ≈–« ﬂ«‰ „ÊÃÊœ«
    try
    {
        var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        if (File.Exists(xmlPath))
        {
            options.IncludeXmlComments(xmlPath);
        }
    }
    catch (Exception ex)
    {
        //  Ã«Â· «·Œÿ√ ≈–« ·„ ÌÊÃœ „·› XML
        Console.WriteLine($"XML documentation file not found: {ex.Message}");
    }
});

// --------------------------------------------------
// DbContext
// --------------------------------------------------
builder.Services.AddDbContext<FormBuilderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// --------------------------------------------------
// Identity Configuration
// --------------------------------------------------
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<FormBuilderDbContext>()
.AddDefaultTokenProviders();

// --------------------------------------------------
// JWT Authentication
// --------------------------------------------------
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = jwtSettings["Key"] ?? throw new InvalidOperationException("JWT Key is not configured");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// --------------------------------------------------
// CORS
// --------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --------------------------------------------------
// Dependency Injection (Services) - VERIFIED
// --------------------------------------------------
// Auth Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IPermissionService, PermissionService>();
builder.Services.AddScoped<IUserService, UserService>();

// Form Builder Services
builder.Services.AddScoped<IFormBuilderService, FormBuilderService>();
builder.Services.AddScoped<IFormBuilderRepository, FormBuilderRepository>();
builder.Services.AddScoped<IFormTabService, FormTabService>();
builder.Services.AddScoped<IFormTabRepository, FormTabRepository>();
builder.Services.AddScoped<IFormFieldService, FormFieldService>();
builder.Services.AddScoped<IFormFieldRepository, FormFieldRepository>();
builder.Services.AddScoped<IFieldTypesService, FieldTypesService>();
builder.Services.AddScoped<IFieldTypesRepository, FieldTypesRepository>();
builder.Services.AddScoped<IFORM_RULESService, FORM_RULESService>();
builder.Services.AddScoped<IFORM_RULESRepository, FORM_RULESRepository>();
builder.Services.AddScoped<IFieldOptionsService, FieldOptionsService>();
builder.Services.AddScoped<IFieldOptionsRepository, FieldOptionsRepository>();
builder.Services.AddScoped<IFieldDataSourcesService, FieldDataSourcesService>();
builder.Services.AddScoped<IFieldDataSourcesRepository, FieldDataSourcesRepository>();
builder.Services.AddScoped<IFormSubmissionsRepository, FormSubmissionsRepository>();
builder.Services.AddScoped<IFormSubmissionsService, FormSubmissionsService>();

// Document & Attachment Services
builder.Services.AddScoped<IAttachmentTypeService, AttachmentTypeService>();
builder.Services.AddScoped<IAttachmentTypeRepository, AttachmentTypeRepository>();
builder.Services.AddScoped<IFormAttachmentTypeService, FormAttachmentTypeService>();
builder.Services.AddScoped<IFormAttachmentTypeRepository, FormAttachmentTypeRepository>();
builder.Services.AddScoped<IDocumentTypeService, DocumentTypeService>();
builder.Services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
// Project Services
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
// ›Ì „·› Startup.cs √Ê Program.cs
builder.Services.AddScoped<IDocumentSeriesRepository, DocumentSeriesRepository>();
builder.Services.AddScoped<IDocumentSeriesService, DocumentSeriesService>();
builder.Services.AddScoped<IFormSubmissionValuesRepository, FormSubmissionValuesRepository>();
builder.Services.AddScoped<IFormSubmissionValuesService, FormSubmissionValuesService>();
builder.Services.AddScoped<IFormSubmissionAttachmentsRepository, FormSubmissionAttachmentsRepository>();
builder.Services.AddScoped<IFormSubmissionAttachmentsService, FormSubmissionAttachmentsService>();
builder.Services.AddScoped<IFormGridRepository, FormGridRepository>();
builder.Services.AddScoped<IFormGridService, FormGridService>();
builder.Services.AddScoped<IFormGridColumnRepository, FormGridColumnRepository>();
builder.Services.AddScoped<IFormGridColumnService, FormGridColumnService>();
builder.Services.AddScoped<IFormSubmissionGridRowRepository, FormSubmissionGridRowRepository>();
builder.Services.AddScoped<IFormSubmissionGridRowService, FormSubmissionGridRowService>();
builder.Services.AddScoped<IFormSubmissionGridCellRepository, FormSubmissionGridCellRepository>();
builder.Services.AddScoped<IFormSubmissionGridCellService, FormSubmissionGridCellService>();
builder.Services.AddScoped<IFormulasRepository, FormulasRepository>();
builder.Services.AddScoped<IFormulaService, FormulaService>();


// Unit of Work -  √ﬂœ „‰ √‰ «·«”„ ’ÕÌÕ
builder.Services.AddScoped<IunitOfwork, UnitOfWork>();

var app = builder.Build();

// --------------------------------------------------
// Middleware Pipeline - FIXED ORDER
// --------------------------------------------------

// «·«” À‰«¡«  ›Ì «·»Ì∆… «· ÿÊÌ—Ì…
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

// Swagger ›Ì Ã„Ì⁄ «·»Ì∆«  √Ê «· ÿÊÌ— ›ﬁÿ
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "FormBuilder API v1");
    options.RoutePrefix = "swagger"; // «·Ê’Ê· ⁄»— /swagger
});

// «· — Ì» «·’ÕÌÕ ··‹ Middleware
app.UseHttpsRedirection();
app.UseCors();
app.UseRouting(); // √÷› Â–« «·”ÿ—

// «·„Â„: Authentication ﬁ»· Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// --------------------------------------------------
// Database Seeding
// --------------------------------------------------
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var context = services.GetRequiredService<FormBuilderDbContext>();
        var userManager = services.GetRequiredService<UserManager<AppUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        //  √ﬂœ „‰ ÊÃÊœ Â–Â «·œ«·…
        await DataSeeder.SeedAsync(context, userManager, roleManager);
        Console.WriteLine("Database seeding completed successfully!");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database");
    }
}

app.Run();