using FormBuilder.Application.Dtos.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace FormBuilder.Application.Abstractions;

public interface IaccountService
{
    Task<LoginResponseDto> LoginAsync(string username, string password, CancellationToken cancellationToken);
    Task<RefreshTokenResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken);
    Task<bool> LogoutAsync(string refreshToken, CancellationToken cancellationToken);
    Task<bool> RevokeAllUserTokensAsync(int userId, CancellationToken cancellationToken);
}
