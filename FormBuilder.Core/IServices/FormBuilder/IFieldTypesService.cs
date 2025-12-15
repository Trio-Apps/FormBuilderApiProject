using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Domian.Entitys.FormBuilder;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

public interface IFieldTypesService
{
    Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetAllAsync(Expression<System.Func<FIELD_TYPES, bool>>? filter = null);
    Task<ServiceResult<PagedResult<FieldTypeDto>>> GetPagedAsync(int page = 1, int pageSize = 20, Expression<System.Func<FIELD_TYPES, bool>>? filter = null);
    Task<ServiceResult<FieldTypeDto>> GetByIdAsync(int id, bool asNoTracking = false);
    Task<ServiceResult<FieldTypeDto>> CreateAsync(FieldTypeCreateDto dto);
    Task<ServiceResult<FieldTypeDto>> UpdateAsync(int id, FieldTypeUpdateDto dto);
    Task<ServiceResult<bool>> DeleteAsync(int id);
    Task<ServiceResult<bool>> SoftDeleteAsync(int id);

    Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetActiveAsync();
    Task<ServiceResult<FieldTypeDto>> GetByTypeNameAsync(string typeName, bool asNoTracking = false);
    Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetWithOptionsAsync();
    Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetByDataTypeAsync(string dataType);
    Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetWithMultipleValuesAsync();
    Task<ServiceResult<IEnumerable<FieldTypeDropdownDto>>> GetForDropdownAsync();
    Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetBasicAsync();
    Task<ServiceResult<IEnumerable<FieldTypeDto>>> GetAdvancedAsync();

    Task<ServiceResult<bool>> IsTypeNameUniqueAsync(string typeName, int? ignoreId = null);
    Task<ServiceResult<bool>> ExistsAsync(int id);
    Task<ServiceResult<int>> GetUsageCountAsync(int fieldTypeId);
}
