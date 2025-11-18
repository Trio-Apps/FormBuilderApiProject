using FormBuilder.Application.DTOS;
using FormBuilder.Application.DTOS.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Application.IServices
{
    public interface IRoleService
    {
        Task<ServiceResult<RoleDto>> GetRoleByIdAsync(int roleId);
        Task<ServiceResult<List<RoleDto>>> GetAllRolesAsync();
        Task<ServiceResult<RoleDto>> CreateRoleAsync(CreateRoleDto createRoleDto);
        Task<ServiceResult<RoleDto>> UpdateRoleAsync(int roleId, UpdateRoleDto updateRoleDto);
        Task<ServiceResult<bool>> DeleteRoleAsync(int roleId);
        Task<ServiceResult<bool>> AssignPermissionsToRoleAsync(int roleId, List<int> permissionIds);
        Task<ServiceResult<bool>> RemovePermissionsFromRoleAsync(int roleId, List<int> permissionIds);
    }

}
