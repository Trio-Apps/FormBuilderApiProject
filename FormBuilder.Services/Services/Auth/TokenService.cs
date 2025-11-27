// Services/TokenService.cs
using FormBuilder.API.Models;
using FormBuilder.Core.IServices.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class TokenService : ITokenService
{
    private readonly IConfiguration _configuration;
    private readonly UserManager<AppUser> _userManager;
    private readonly SymmetricSecurityKey _key;

    public TokenService(IConfiguration configuration, UserManager<AppUser> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
        _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
    }

    public async Task<string> CreateTokenAsync(AppUser user)
    {
        var (token, _) = await CreateTokenWithIdAsync(user);
        return token;
    }

    public async Task<(string Token, string JwtId)> CreateTokenWithIdAsync(AppUser user)
    {
        var jwtId = Guid.NewGuid().ToString();

        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.GivenName, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, jwtId) // مهم للربط مع Refresh Token
        };

        var roles = await _userManager.GetRolesAsync(user);
        foreach (var role in roles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var signingCredentials = new SigningCredentials(_key, SecurityAlgorithms.HmacSha256);

        int expirationMinutes = int.TryParse(_configuration["Jwt:ExpiryInMinutes"], out int exp)
            ? exp : 15;

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: authClaims,
            expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
            signingCredentials: signingCredentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
        return (tokenString, jwtId);
    }

    public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
    {
        try
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _key,
                ValidateLifetime = false // نتجاهل انتهاء الصلاحية
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);

            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
        catch
        {
            return null;
        }
    }

    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}