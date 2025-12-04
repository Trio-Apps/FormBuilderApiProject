using FormBuilder.Application.Abstractions;
using FormBuilder.Application.Dtos.Auth;
using FormBuilder.Core.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

    public async Task<LoginResponseDto> LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _context.TblUsers
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password, cancellationToken);

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
            Expires = now.AddMinutes(expiryMinutes).AddSeconds(1), // فرق بسيط لتجنب IDX12401
            SigningCredentials = creds,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return new LoginResponseDto
        {
            Success = true,
            Token = tokenString,
            Role = roleName,
            ExpiresAt = tokenDescriptor.Expires!.Value
        };
    }
}
