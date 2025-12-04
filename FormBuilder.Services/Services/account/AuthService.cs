using FormBuilder.API.Models;
using FormBuilder.Application.Abstractions;
using FormBuilder.core.DTOS.Auth;
using FormBuilder.Core.DTOS.Auth;
using FormBuilder.Core.Models;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FormBuilder.Infrastructure.Services;

public class accoutService : IaccountService
{
    private readonly AkhmanageItContext _context;
    private readonly IServer _server;

    private const string JwtSecret = "99n6tDRTzftaPXYI8/ohgs0WsMWS1Yd9JuY=";
    private const string Audience = "FormBuilderClients";
    private const int TokenExpiryMinutes = 60;

    public accoutService(AkhmanageItContext context, IServer server)
    {
        _context = context;
        _server = server;
    }

    public async Task<string?> LoginAsync(string username, string password, CancellationToken cancellationToken)
    {
        var user = await _context.TblUsers
            .FirstOrDefaultAsync(x => x.Username == username && x.Password == password, cancellationToken);

        if (user is null)
            return null;

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.Username),
        ];

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(JwtSecret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(TokenExpiryMinutes),
            SigningCredentials = credentials,
            Issuer = _server.Features.Get<IServerAddressesFeature>()!.Addresses.First(),
            Audience = Audience
        };

        var handler = new JwtSecurityTokenHandler();
        var token = handler.CreateToken(tokenDescriptor);

        return handler.WriteToken(token);
    }


}
