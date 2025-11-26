using FormBuilder.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Core.IServices.FormBuilder
{
    public interface IFieldOptionsService
    {
        // Add this method
        Task<ApiResponse> GetAllAsync();

        // Existing methods
        Task<ApiResponse> GetByFieldIdAsync(int fieldId);
        Task<ApiResponse> GetActiveByFieldIdAsync(int fieldId);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> CreateAsync(CreateFieldOptionDto createDto);
        Task<ApiResponse> CreateBulkAsync(List<CreateFieldOptionDto> createDtos);
        Task<ApiResponse> UpdateAsync(int id, UpdateFieldOptionDto updateDto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> SoftDeleteAsync(int id);
        Task<ApiResponse> GetDefaultOptionAsync(int fieldId);
        Task<ApiResponse> GetOptionsCountAsync(int fieldId);
    }
}