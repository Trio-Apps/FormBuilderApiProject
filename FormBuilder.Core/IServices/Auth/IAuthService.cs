using FormBuilder.Application.Dtos.Auth;
using System.Threading;
using System.Threading.Tasks;

namespace FormBuilder.Application.Abstractions;

public interface IaccountService
{
    Task<LoginResponseDto> LoginAsync(string username, string password, CancellationToken cancellationToken);
}
