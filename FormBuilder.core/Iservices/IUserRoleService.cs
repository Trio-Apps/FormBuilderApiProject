using FormBuilder.Application.DTOS;
using FormBuilder.Application.DTOS.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Application.IServices
{

    // Services/IUserRoleService.cs
    public interface IUserRoleService
    {
        Task<ServiceResult<bool>> AssignRoleToUserAsync(AssignRoleToUserDto assignRoleDto);
        Task<ServiceResult<bool>> RemoveRoleFromUserAsync(string userId, int roleId);
        Task<ServiceResult<List<RoleDto>>> GetUserRolesAsync(string userId);
        Task<ServiceResult<List<UserWithRolesDto>>> GetUsersWithRolesAsync();
        Task<ServiceResult<bool>> UpdateUserRoleAsync(string userId, int roleId, DateTime? endDate);
    }
}
