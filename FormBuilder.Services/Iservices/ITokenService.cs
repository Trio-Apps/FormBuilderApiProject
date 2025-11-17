// Services/ITokenService.cs
using FormBuilder.API.Models;

namespace FormBuilder.API.Services
{
    public interface ITokenService
    {
        Task<string> CreateTokenAsync(User user);
    }
}