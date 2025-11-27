using FormBuilder.API.Models;
using FormBuilder.API.Models.DTOs;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Services
{
    public interface IDocumentTypeService
    {
        Task<ApiResponse> GetAllAsync();
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetByCodeAsync(string code);
        Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId);
        Task<ApiResponse> GetActiveAsync();
        Task<ApiResponse> GetByParentMenuIdAsync(int? parentMenuId);
        Task<ApiResponse> CreateAsync(CreateDocumentTypeDto createDto);
        Task<ApiResponse> UpdateAsync(int id, UpdateDocumentTypeDto updateDto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> ToggleActiveAsync(int id, bool isActive);
        Task<ApiResponse> ExistsAsync(int id);
        Task<ApiResponse> CodeExistsAsync(string code, int? excludeId = null);
    }
}