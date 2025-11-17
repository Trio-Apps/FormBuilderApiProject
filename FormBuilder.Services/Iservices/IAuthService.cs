



using FormBuilder.API.DTOs;
using FormBuilder.API.DTOs.FormBuilder.API.DTOs;
using FormBuilder.API.Models;

namespace FormBuilder.API.Services
{
    public interface IAuthService
    {
        Task<SingleApiResponse> RegisterAsync(UserRegisterDto userRegister, string origin);
        Task<SingleApiResponse> Login(UserLoginDto userLoginDto);
        //Task<SingleApiResponse> ForgetPasswordAsync(ForgetPasswordDto dto);
        //Task<SingleApiResponse> ResetPasswordAsync(ResetPasswordDto dto);
        //Task<SingleApiResponse> ChangePasswordAsync(ChangePasswordDto dto);
        Task<SingleApiResponse> RefreshTokenAsync(string refreshToken);
        Task<SingleApiResponse> LogoutAsync(int userId);
    }
}