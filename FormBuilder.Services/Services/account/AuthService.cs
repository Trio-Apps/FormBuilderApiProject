using FormBuilder.Application.Abstractions;
using FormBuilder.Application.Dtos.Auth;
using FormBuilder.Core.Models;
using formBuilder.Domian.Entitys;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

public class accountService : IaccountService
{
    private readonly AkhmanageItContext _context;
    private readonly IServer _server;
    private readonly IConfiguration _configuration;

    public accountService(AkhmanageItContext context, IServer server, IConfiguration configuration)
    {
        _context = context;
        _server = server;
        _configuration = configuration;
    }

    // دالة SHA512
    private static string HashPassword(string password)
    {
        using (SHA512 sha = SHA512.Create())
        {
            byte[] bytes = Encoding.UTF8.GetBytes(password);
            byte[] hash = sha.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder(128);
            foreach (byte b in hash)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }
    }

    public async Task<LoginResponseDto> LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        string hashedPassword = HashPassword(password);

        // ابحث عن المستخدم باستخدام الـ username و الـ hashed password
        var user = await _context.TblUsers
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == hashedPassword, cancellationToken);

        if (user == null)
            return new LoginResponseDto { Success = false, ErrorMessage = "Invalid username or password." };

        // نجيب الدور من جدول TblUserGroup باستخدام GroupId فقط
        var group = await _context.TblUserGroups
            .FirstOrDefaultAsync(g => g.Id == user.Id, cancellationToken);

        var roleName = group?.Name ?? "User"; // Default role

        var now = DateTime.UtcNow;
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, roleName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiryMinutes = Convert.ToDouble(jwtSettings["ExpiryMinutes"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = now,
            Expires = now.AddMinutes(expiryMinutes).AddSeconds(1),
            SigningCredentials = creds,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Generate refresh token
        var refreshToken = GenerateRefreshToken();
        var refreshTokenExpiryDays = int.TryParse(_configuration.GetSection("Jwt")["RefreshTokenExpiryDays"], out var days) ? days : 7;
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

        // Save refresh token to database
        var refreshTokenEntity = new REFRESH_TOKENS
        {
            UserId = user.Id,
            Token = refreshToken,
            ExpiresAt = refreshTokenExpiresAt,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.REFRESH_TOKENS.Add(refreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        // Revoke old refresh tokens for this user (keep only the latest 5)
        await RevokeOldRefreshTokensAsync(user.Id, cancellationToken);

        return new LoginResponseDto
        {
            Success = true,
            Token = tokenString,
            RefreshToken = refreshToken,
            Role = roleName,
            ExpiresAt = tokenDescriptor.Expires!.Value,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenEntity = await _context.REFRESH_TOKENS
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken && rt.IsActive, cancellationToken);

        if (tokenEntity == null || tokenEntity.ExpiresAt < DateTime.UtcNow || tokenEntity.RevokedAt != null)
        {
            return new RefreshTokenResponseDto
            {
                Success = false,
                ErrorMessage = "Invalid or expired refresh token."
            };
        }

        // Get user
        var user = await _context.TblUsers
            .FirstOrDefaultAsync(u => u.Id == tokenEntity.UserId, cancellationToken);

        if (user == null)
        {
            return new RefreshTokenResponseDto
            {
                Success = false,
                ErrorMessage = "User not found."
            };
        }

        // Get user role
        var group = await _context.TblUserGroups
            .FirstOrDefaultAsync(g => g.Id == user.Id, cancellationToken);
        var roleName = group?.Name ?? "User";

        // Generate new access token
        var now = DateTime.UtcNow;
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, roleName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var expiryMinutes = Convert.ToDouble(jwtSettings["ExpiryMinutes"]);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            NotBefore = now,
            Expires = now.AddMinutes(expiryMinutes).AddSeconds(1),
            SigningCredentials = creds,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        // Optionally rotate refresh token (generate new one and revoke old)
        var newRefreshToken = GenerateRefreshToken();
        var refreshTokenExpiryDays = int.TryParse(_configuration.GetSection("Jwt")["RefreshTokenExpiryDays"], out var days) ? days : 7;
        var refreshTokenExpiresAt = DateTime.UtcNow.AddDays(refreshTokenExpiryDays);

        // Revoke old refresh token
        tokenEntity.RevokedAt = DateTime.UtcNow;
        tokenEntity.IsActive = false;

        // Create new refresh token
        var newRefreshTokenEntity = new REFRESH_TOKENS
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = refreshTokenExpiresAt,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.REFRESH_TOKENS.Add(newRefreshTokenEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new RefreshTokenResponseDto
        {
            Success = true,
            Token = tokenString,
            RefreshToken = newRefreshToken,
            ExpiresAt = tokenDescriptor.Expires!.Value,
            RefreshTokenExpiresAt = refreshTokenExpiresAt
        };
    }

    public async Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var tokenEntity = await _context.REFRESH_TOKENS
            .FirstOrDefaultAsync(rt => rt.Token == refreshToken, cancellationToken);

        if (tokenEntity != null)
        {
            tokenEntity.RevokedAt = DateTime.UtcNow;
            tokenEntity.IsActive = false;
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        return false;
    }

    public async Task<bool> RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken)
    {
        var activeTokens = await _context.REFRESH_TOKENS
            .Where(rt => rt.UserId == userId && rt.IsActive && rt.RevokedAt == null)
            .ToListAsync(cancellationToken);

        foreach (var token in activeTokens)
        {
            token.RevokedAt = DateTime.UtcNow;
            token.IsActive = false;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    private string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    private async Task RevokeOldRefreshTokensAsync(int userId, CancellationToken cancellationToken)
    {
        // Keep only the latest 5 active refresh tokens per user
        var activeTokens = await _context.REFRESH_TOKENS
            .Where(rt => rt.UserId == userId && rt.IsActive && rt.RevokedAt == null)
            .OrderByDescending(rt => rt.CreatedAt)
            .ToListAsync(cancellationToken);

        if (activeTokens.Count > 5)
        {
            var tokensToRevoke = activeTokens.Skip(5);
            foreach (var token in tokensToRevoke)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.IsActive = false;
            }
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
