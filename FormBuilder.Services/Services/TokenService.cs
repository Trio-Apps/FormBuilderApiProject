// Services/TokenService.cs
using FormBuilder.API.Data;
using FormBuilder.API.Models;
using FormBuilder.API.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly AuthDbContext _context;

    public TokenService(IConfiguration configuration, AuthDbContext context)
    {
        _configuration = configuration;
        _context = context;
    }

    public async Task<string> CreateTokenAsync(User user)
    {
        // الحصول على الأدوار من قاعدة البيانات
        var userWithRoles = await _context.USERS
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        var roles = userWithRoles?.UserRoles
            .Where(ur => ur.IsActive && ur.Role.IsActive)
            .Select(ur => ur.Role.RoleName)
            .ToList() ?? new List<string>();

        return GenerateToken(user, roles);
    }

    public string GenerateToken(User user, List<string> roles)
    {
        // Set claims
        var authClaims = new List<Claim>
        {
            new Claim("uid", user.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email ?? ""),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add roles to claims
        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Get JWT configuration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // Parse expiration
        var expirationMinutesStr = _configuration["JwtSettings:DurationInMinutes"];
        if (!int.TryParse(expirationMinutesStr, out int expirationMinutes))
        {
            expirationMinutes = 60;
        }

        // Create token
        var token = new JwtSecurityToken(
            issuer: _configuration["JwtSettings:Issuer"],
            audience: _configuration["JwtSettings:Audience"],
            claims: authClaims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: signingCredentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}