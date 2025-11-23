using FormBuilder.Application.DTOS;
using FormBuilder.Application.DTOS.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.Application.IServices
{
    public interface IPermissionService
    {
        Task<ServiceResult<PermissionDto>> GetPermissionByIdAsync(int permissionId);
        Task<ServiceResult<List<PermissionDto>>> GetAllPermissionsAsync();
        Task<ServiceResult<PermissionDto>> CreatePermissionAsync(CreatePermissionDto createPermissionDto);
        Task<ServiceResult<PermissionDto>> UpdatePermissionAsync(int permissionId, UpdatePermissionDto updatePermissionDto);
        Task<ServiceResult<bool>> DeletePermissionAsync(int permissionId);
    }
}
