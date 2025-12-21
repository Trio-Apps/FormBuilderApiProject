using FormBuilder.Infrastructure.Data;
using FormBuilder.API.ExceptionHandlers;
using FormBuilder.API.Extensions;
using FormBuilder.API.HealthChecks;
using FormBuilder.Core.Models;
using FormBuilder.Core.Configuration;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.Json;
using FormBuilder.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Localization
// -----------------------------
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// -----------------------------
// Controllers + JSON + ProblemDetails
// -----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();

builder.Services.AddEndpointsApiExplorer();

// Add ProblemDetails service for exception handling
builder.Services.AddProblemDetails();

// -----------------------------
// Swagger Configuration (بسيط كما كان سابقاً)
// -----------------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "FormBuilder API",
        Version = "v1",
        Description = "API for FormBuilder Application"
    });

    // Use fully qualified type names for schema IDs to avoid conflicts
    c.CustomSchemaIds(type => 
    {
        if (type == null) return null;
        
        // Handle generic types
        if (type.IsGenericType)
        {
            var genericTypeName = type.GetGenericTypeDefinition().Name;
            var genericArgs = string.Join("_", type.GetGenericArguments().Select(t => t.Name));
            return $"{genericTypeName}_{genericArgs}";
        }
        
        var name = type.FullName ?? type.Name;
        return name?.Replace("+", ".");
    });

    // Ignore IActionResult and File results for Swagger schema generation
    c.MapType<IActionResult>(() => new OpenApiSchema { Type = "object" });
    c.MapType<FileResult>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    c.MapType<FileStreamResult>(() => new OpenApiSchema { Type = "string", Format = "binary" });
    
    // Ignore obsolete items to avoid schema generation issues
    c.IgnoreObsoleteActions();
    c.IgnoreObsoleteProperties();
    
    // Suppress schema generation errors
    c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

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
    var connectionString = builder.Configuration.GetConnectionString("AuthConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = "Server=DESKTOP-B3NJLJM;Database=AkhmanageItDb;Trusted_Connection=True;TrustServerCertificate=True;";
        Console.WriteLine("Using default Auth connection string");
    }

    options.UseSqlServer(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

// Business / Forms DbContext
builder.Services.AddDbContext<FormBuilderDbContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = "Server=DESKTOP-B3NJLJM;Database=FormBuilderDb;Trusted_Connection=True;TrustServerCertificate=True;";
        Console.WriteLine("Using default FormBuilder connection string");
    }

    options.UseSqlServer(connectionString);

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
        options.EnableSensitiveDataLogging();
    }
});

// -----------------------------
// Caching
// -----------------------------
builder.Services.AddMemoryCache();

// -----------------------------
// Health Checks
// -----------------------------
builder.Services.AddHealthChecks()
    .AddCheck<FormBuilderDbHealthCheck>("formbuilder-db", tags: new[] { "db", "ready" })
    .AddCheck<AuthDbHealthCheck>("auth-db", tags: new[] { "db", "ready" });

// -----------------------------
// Response Compression
// -----------------------------
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.BrotliCompressionProvider>();
    options.Providers.Add<Microsoft.AspNetCore.ResponseCompression.GzipCompressionProvider>();
});

// -----------------------------
// Dependency Injection
// -----------------------------

builder.Services.AddFormBuilderServices();


// Register GlobalExceptionHandler
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

// -----------------------------
// JWT Authentication
// -----------------------------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var key = jwtSettings["Key"];

if (string.IsNullOrEmpty(key))
{
    key = "99n6tDRTzftaPXYI8/ohgs0WsMWS1Yd9JuY=";
    Console.WriteLine("Warning: Using default JWT key");
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
        }
    };
});

// -----------------------------
// Rate Limiting Configuration
// -----------------------------
builder.Services.Configure<RateLimitingOptions>(
    builder.Configuration.GetSection(RateLimitingOptions.SectionName));

// -----------------------------
// CORS - Improved Security
// -----------------------------
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:3000", "http://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Important for cookies/auth headers
    });

    // Keep AllowAll for development only
    if (builder.Environment.IsDevelopment())
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
    }
});

// -----------------------------
// Build Application
// -----------------------------
var app = builder.Build();

// -----------------------------
// Middleware Pipeline + Localization
// -----------------------------

var supportedCultures = new[]
{
    new CultureInfo("en"),
    new CultureInfo("ar")
};

app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new RequestCulture("en"),
    SupportedCultures = supportedCultures,
    SupportedUICultures = supportedCultures
});

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    
    app.UseSwaggerUI(c =>
    {
        // Swagger endpoint for v1 (كما كان سابقاً)
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FormBuilder API v1");

        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
        c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List);
        c.EnableDeepLinking();
        c.EnableFilter();
        c.ShowExtensions();
        c.EnableValidator();
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}




app.UseHttpsRedirection();

// Response Compression
app.UseResponseCompression();

// Add Rate Limiting Middleware (قبل Routing)
app.UseRateLimiting();

app.UseRouting();

// CORS - Use specific origins in production, AllowAll in development
app.UseCors(builder.Environment.IsDevelopment() ? "AllowAll" : "AllowSpecificOrigins");

// Add exception handling middleware
app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

// Error handling endpoint
app.Map("/error", ap => ap.Run(async context =>
{
    context.Response.StatusCode = 500;
    await context.Response.WriteAsync("An error occurred. Please contact administrator.");
}));



app.MapControllers();

// Health Check Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                exception = e.Value.Exception?.Message,
                duration = e.Value.Duration.ToString()
            })
        });
        await context.Response.WriteAsync(result);
    }
});

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = _ => false // No checks for liveness, just returns 200 if app is running
});

app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("FormBuilder API started successfully");
});

app.Run();