using formBuilder.Domian.Interfaces;
using FormBuilder.API.Data;
using FormBuilder.API.Models;
using FormBuilder.API.Services;
using FormBuilder.core.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Debug);

// Controllers & JSON options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -------------------------
// DbContexts
// -------------------------
// NOTE: Use the real assembly name for migrations or detect it dynamically:
var authMigrationsAssembly = typeof(AuthDbContext).Assembly.GetName().Name;
var appMigrationsAssembly = typeof(AppDbContext).Assembly.GetName().Name;

builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("AuthConnection"),
        sql => sql.MigrationsAssembly(authMigrationsAssembly)
    );

    // ›ﬁÿ ›Ì »Ì∆… «· ÿÊÌ— ó ·«  ›⁄¯· sensitive logging ›Ì «·«‰ «Ã
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.MigrationsAssembly(appMigrationsAssembly)
    );

    if (builder.Environment.IsDevelopment())
    {
        options.EnableDetailedErrors();
    }
});

// -------------------------
// Identity („·«ÕŸ… „Â„… ÕÊ· «” Œœ«„ int ﬂ‹ key)
// -------------------------
// ≈–« ﬂ‰   ” Œœ„ „› «Õ „‰ «·‰Ê⁄ int:  √ﬂœ √‰ User Ì—À IdentityUser<int>
// Ê√‰ AuthDbContext Ì—À IdentityDbContext<User, IdentityRole<int>, int>
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;

    options.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

// -------------------------
// CORS
// -------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// -------------------------
// App services / DI
// -------------------------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();

// If you have UnitOfWork & repository pattern:
builder.Services.AddScoped<IunitOfwork, UnitOfWork>();

var app = builder.Build();

// Dev-only middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseDeveloperExceptionPage();
}

app.UseCors();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
