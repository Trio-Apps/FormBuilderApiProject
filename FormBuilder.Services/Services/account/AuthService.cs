using FormBuilder.API.Models;
using FormBuilder.Application.Abstractions;
using FormBuilder.Core.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FormBuilder.Infrastructure.Services;

public class accountService : IaccountService
{
    private readonly AkhmanageItContext _context;
    private readonly IServer _server;
    private readonly IConfiguration _configuration;

    public accountService(
        AkhmanageItContext context,
        IServer server,
        IConfiguration configuration)
    {
        _context = context;
        _server = server;
        _configuration = configuration;
    }

    public async Task<string?> LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _context.TblUsers
            .FirstOrDefaultAsync(x => x.Username == username && x.Password == password, cancellationToken);

        if (user is null)
            return null;

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // احصل على الإعدادات من appsettings.json
        var jwtSettings = _configuration.GetSection("Jwt");
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60), // صلاحية لمدة ساعة
            SigningCredentials = credentials,
            Issuer = jwtSettings["Issuer"],
            Audience = jwtSettings["Audience"]
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }
}