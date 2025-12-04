using System.Threading;
using System.Threading.Tasks;

namespace FormBuilder.Application.Abstractions;

public interface IaccountService
{
    Task<string?> LoginAsync(string username, string password, CancellationToken cancellationToken);
}
