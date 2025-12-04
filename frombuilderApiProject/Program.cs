using formBuilder.Domian.Interfaces;
using FormBuilder.API.Data;
using FormBuilder.Application.Abstractions;
using FormBuilder.core.Repository;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Core.IServices.FormBuilder.FormBuilder.Services.Services;
using FormBuilder.Core.Models;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Interfaces;
using FormBuilder.Infrastructure.Repositories;
using FormBuilder.Infrastructure.Repository;
using FormBuilder.Infrastructure.Services;
using FormBuilder.Services;
using FormBuilder.Services.Repository;
using FormBuilder.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Controllers + JSON
// -----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();

// -----------------------------
// Swagger Configuration with JWT Support
// -----------------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FormBuilder API",
        Version = "v1",
        Description = "API for FormBuilder Application"
    });

    // Add JWT Authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your token"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// -----------------------------
// Database Contexts
// -----------------------------

// Security / Auth DbContext
builder.Services.AddDbContext<AkhmanageItContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("AuthConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

// Business / Forms DbContext
builder.Services.AddDbContext<FormBuilderDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

// -----------------------------
// Dependency Injection - Services Registration
// -----------------------------

// Account Service
builder.Services.AddScoped<IaccountService, accountService>();

// Unit of Work
builder.Services.AddScoped<IunitOfwork, UnitOfWork>();

// Form Builder Services
builder.Services.AddScoped<IFormBuilderService, FormBuilderService>();
builder.Services.AddScoped<IFormBuilderRepository, FormBuilderRepository>();

// Form Tab Services
builder.Services.AddScoped<IFormTabService, FormTabService>();
builder.Services.AddScoped<IFormTabRepository, FormTabRepository>();

// Form Field Services
builder.Services.AddScoped<IFormFieldService, FormFieldService>();
builder.Services.AddScoped<IFormFieldRepository, FormFieldRepository>();

// Field Types Services
builder.Services.AddScoped<IFieldTypesService, FieldTypesService>();
builder.Services.AddScoped<IFieldTypesRepository, FieldTypesRepository>();

// Other Form Services
builder.Services.AddScoped<IFORM_RULESService, FORM_RULESService>();
builder.Services.AddScoped<IFORM_RULESRepository, FORM_RULESRepository>();
builder.Services.AddScoped<IFieldOptionsService, FieldOptionsService>();
builder.Services.AddScoped<IFieldOptionsRepository, FieldOptionsRepository>();
builder.Services.AddScoped<IFieldDataSourcesService, FieldDataSourcesService>();
builder.Services.AddScoped<IFieldDataSourcesRepository, FieldDataSourcesRepository>();
builder.Services.AddScoped<IFormSubmissionsService, FormSubmissionsService>();
builder.Services.AddScoped<IFormSubmissionsRepository, FormSubmissionsRepository>();

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

// Additional Services
builder.Services.AddScoped<IDocumentSeriesService, DocumentSeriesService>();
builder.Services.AddScoped<IDocumentSeriesRepository, DocumentSeriesRepository>();
builder.Services.AddScoped<IFormSubmissionValuesService, FormSubmissionValuesService>();
builder.Services.AddScoped<IFormSubmissionValuesRepository, FormSubmissionValuesRepository>();
builder.Services.AddScoped<IFormSubmissionAttachmentsService, FormSubmissionAttachmentsService>();
builder.Services.AddScoped<IFormSubmissionAttachmentsRepository, FormSubmissionAttachmentsRepository>();

// Grid Services
builder.Services.AddScoped<IFormGridService, FormGridService>();
builder.Services.AddScoped<IFormGridRepository, FormGridRepository>();
builder.Services.AddScoped<IFormGridColumnService, FormGridColumnService>();
builder.Services.AddScoped<IFormGridColumnRepository, FormGridColumnRepository>();
builder.Services.AddScoped<IFormSubmissionGridRowService, FormSubmissionGridRowService>();
builder.Services.AddScoped<IFormSubmissionGridRowRepository, FormSubmissionGridRowRepository>();
builder.Services.AddScoped<IFormSubmissionGridCellService, FormSubmissionGridCellService>();
builder.Services.AddScoped<IFormSubmissionGridCellRepository, FormSubmissionGridCellRepository>();

// Formulas Service
builder.Services.AddScoped<IFormulaService, FormulaService>();
builder.Services.AddScoped<IFormulasRepository, FormulasRepository>();

// -----------------------------
// JWT Authentication Configuration
// -----------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];

if (string.IsNullOrEmpty(key))
{
    // Use default key for development if not configured
    key = "99n6tDRTzftaPXYI8/ohgs0WsMWS1Yd9JuY=";
    Console.WriteLine("Warning: Using default JWT key. For production, configure JWT:Key in appsettings.json");
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"] ?? "http://localhost:5000",
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"] ?? "FormBuilderClients",
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    // Events for debugging
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            Console.WriteLine($"OnChallenge: {context.Error}, {context.ErrorDescription}");
            return Task.CompletedTask;
        }
    };
});

// -----------------------------
// CORS Configuration
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// -----------------------------
// Build Application
// -----------------------------
var app = builder.Build();

// -----------------------------
// Middleware Pipeline
// -----------------------------

// Development settings
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FormBuilder API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.EnablePersistAuthorization();
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// CORS must come after Routing and before Authentication
app.UseCors("AllowAll");

// Authentication & Authorization
app.UseAuthentication();
app.UseAuthorization();

// Error handling endpoint
app.Map("/error", ap => ap.Run(async context =>
{
    context.Response.StatusCode = 500;
    await context.Response.WriteAsync("An error occurred. Please contact administrator.");
}));

// Health check endpoint
app.MapGet("/health", () => new
{
    Status = "Healthy",
    Timestamp = DateTime.UtcNow,
    Version = "1.0.0"
});

// Map controllers
app.MapControllers();

// Startup log
app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("FormBuilder API started successfully");
    logger.LogInformation("Environment: {Environment}", app.Environment.EnvironmentName);
    logger.LogInformation("Swagger UI: {Url}/swagger", app.Urls.FirstOrDefault() ?? "http://localhost:5000");
});

// Run application
app.Run();