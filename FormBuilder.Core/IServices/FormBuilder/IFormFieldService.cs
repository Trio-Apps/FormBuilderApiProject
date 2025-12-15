using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormFields;
using FormBuilder.Domian.Entitys.FormBuilder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces
{
    public interface IFormFieldService
    {
        // Basic CRUD operations
        Task<ApiResponse> CreateAsync(Core.DTOS.FormFields.CreateFormFieldDto dto);
        Task<ApiResponse> UpdateAsync(UpdateFormFieldDto dto, int id);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> SoftDeleteAsync(int id);
        Task<FormFieldDto> GetByIdAsync(int id);
        Task<IEnumerable<FormFieldDto>> GetAllAsync();

        // Special queries
        Task<IEnumerable<FormFieldDto>> GetActiveAsync();
        Task<IEnumerable<FormFieldDto>> GetByTabIdAsync(int tabId);
        Task<IEnumerable<FormFieldDto>> GetByFormIdAsync(int formBuilderId);
        Task<IEnumerable<FormFieldDto>> GetMandatoryFieldsAsync(int tabId);
        Task<IEnumerable<FormFieldDto>> GetVisibleFieldsAsync(int tabId);
        Task<FormFieldDto> GetByFieldCodeAsync(string fieldCode);
        Task<IEnumerable<FormFieldDto>> GetEditableFieldsAsync(int tabId);

        // Validation
        Task<bool> IsFieldCodeUniqueAsync(string fieldCode, int? ignoreId = null);
        Task<bool> IsFieldNameUniqueAsync(string fieldName, int? ignoreId = null, int? tabId = null);

        // Utility
        Task<bool> ExistsAsync(int id);
        Task<int> GetUsageCountAsync(int fieldId);
        Task<int> GetFieldsCountByTabAsync(int tabId);
        Task<int> GetFieldsCountByFormAsync(int formBuilderId);
    }
}