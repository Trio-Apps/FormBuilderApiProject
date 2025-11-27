using FormBuilder.Application.DTOS;
using FormBuilder.Application.DTOS.Auth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Core.IServices.Auth
{
    public interface IRoleService
    {
        // ✅ Basic Role Operations
        Task<ServiceResult<RoleDto>> GetRoleByIdAsync(string roleId);
        Task<ServiceResult<RoleDto>> GetRoleByNameAsync(string roleName);
        Task<ServiceResult<List<RoleDto>>> GetAllRolesAsync();
        Task<ServiceResult<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<ServiceResult<RoleDto>> UpdateRoleAsync(string roleId, UpdateRoleDto updateRoleDto);
        Task<ServiceResult<bool>> DeleteRoleAsync(string roleId);

        // ✅ Role-Permission Management
        Task<ServiceResult<bool>> AssignPermissionsToRoleAsync(string roleId, List<int> permissionIds);
    }
}