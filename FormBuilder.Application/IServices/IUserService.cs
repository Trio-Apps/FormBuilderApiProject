using FormBuilder.Application.DTOS;
using FormBuilder.Application.DTOS.Auth;
using FormBuilder.core.DTOS.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Application.IServices
{
    public interface IUserService
    {
        // ✅ User Management - استخدام UsersDto بدلاً من UserDto
        Task<ServiceResult<UsersDto>> GetUserByIdAsync(string userId);
        Task<ServiceResult<UsersDto>> GetUserByEmailAsync(string email);
        Task<ServiceResult<List<UsersDto>>> GetAllUsersAsync();
        Task<ServiceResult<List<UsersDto>>> GetActiveUsersAsync();
        Task<ServiceResult<UsersDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<ServiceResult<UsersDto>> UpdateUserAsync(string userId, UpdateUserDto updateUserDto);
        Task<ServiceResult<bool>> DeleteUserAsync(string userId);
        Task<ServiceResult<bool>> DeactivateUserAsync(string userId);
        Task<ServiceResult<bool>> ActivateUserAsync(string userId);

        // ... باقي الدوال
    }
}