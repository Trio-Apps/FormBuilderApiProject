// Services/ITokenService.cs
using FormBuilder.API.Models;
using System.Security.Claims;

namespace FormBuilder.API.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(AppUser user);
        Task<(string Token, string JwtId)> CreateTokenWithIdAsync(AppUser user);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        string GenerateRefreshToken();
    }
}