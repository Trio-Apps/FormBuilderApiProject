using FormBuilder.API.Models;
using FormBuilder.API.Models.FormBuilder.API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IFieldTypesService
{
    // ================================
    // CRUD Operations
    // ================================
    Task<ApiResponse> CreateAsync(FieldTypeCreateDto dto);       // موحد مع ApiResponse
    Task<ApiResponse> UpdateAsync(FieldTypeUpdateDto dto, int id); // موحد مع ApiResponse
    Task<ApiResponse> DeleteAsync(int id);                        // موحد مع ApiResponse
    Task<ApiResponse> SoftDeleteAsync(int id);                    // موحد مع ApiResponse
    Task<FieldTypeDto> GetByIdAsync(int id);
    Task<IEnumerable<FieldTypeDto>> GetAllAsync();

    // ================================
    // Specialized Queries
    // ================================
    Task<IEnumerable<FieldTypeDto>> GetActiveAsync();
    Task<FieldTypeDto> GetByTypeNameAsync(string typeName);
    Task<IEnumerable<FieldTypeDto>> GetWithOptionsAsync();
    Task<IEnumerable<FieldTypeDto>> GetByDataTypeAsync(string dataType);
    Task<IEnumerable<FieldTypeDto>> GetWithMultipleValuesAsync();
    Task<IEnumerable<FieldTypeDropdownDto>> GetForDropdownAsync();
    Task<IEnumerable<FieldTypeDto>> GetBasicAsync();
    Task<IEnumerable<FieldTypeDto>> GetAdvancedAsync();

    // ================================
    // Validation
    // ================================
    Task<bool> IsTypeNameUniqueAsync(string typeName, int? ignoreId = null);

    // ================================
    // Utility
    // ================================
    Task<bool> ExistsAsync(int id);
    Task<int> GetUsageCountAsync(int fieldTypeId);
}
