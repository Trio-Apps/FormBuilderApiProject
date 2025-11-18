using FormBuilder.API.Models;
using FormBuilder.API.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;

    public TokenService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> CreateTokenAsync(AppUser user)
    {
        // Set claims - استخدم user بدلاً من AppUser
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),  // user ID claim
            new Claim(ClaimTypes.GivenName, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) // unique token ID
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expirationMinutesStr = _configuration["Jwt:ExpiryInMinutes"];

        if (!int.TryParse(expirationMinutesStr, out int expirationMinutes))
        {
            expirationMinutes = 60; // default value
        }

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: authClaims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: signingCredentials
        );

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }
}