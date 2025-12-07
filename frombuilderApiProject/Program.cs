using FormBuilder.API.Data;
using FormBuilder.API.ExceptionHandlers;
using FormBuilder.API.Extensions;
using FormBuilder.Core.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Controllers + JSON + ProblemDetails
// -----------------------------
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();

// Add ProblemDetails service for exception handling
builder.Services.AddProblemDetails();

// -----------------------------
// Swagger Configuration
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
// CORS
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

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "FormBuilder API v1");
        c.RoutePrefix = "swagger";
        c.DisplayRequestDuration();
    });
}
else
{
    app.UseExceptionHandler("/error");
    app.UseHsts();
}




app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("AllowAll");

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

app.Lifetime.ApplicationStarted.Register(() =>
{
    var logger = app.Services.GetRequiredService<ILogger<Program>>();
    logger.LogInformation("FormBuilder API started successfully");
});

app.Run();