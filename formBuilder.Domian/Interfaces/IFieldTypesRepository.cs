using formBuilder.Domian.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces
{
    public interface IFieldTypesRepository : IBaseRepository<FIELD_TYPES>
    {
        // Get all active field types
        Task<IEnumerable<FIELD_TYPES>> GetActiveFieldTypesAsync();

        // Get field type by type name
        Task<FIELD_TYPES> GetByTypeNameAsync(string typeName);

        // Get field types that support options
        Task<IEnumerable<FIELD_TYPES>> GetFieldTypesWithOptionsAsync();

        // Get field types by data type
        Task<IEnumerable<FIELD_TYPES>> GetByDataTypeAsync(string dataType);

        // Check if type name is unique
        Task<bool> IsTypeNameUniqueAsync(string typeName, int? ignoreId = null);

        // Get field types that allow multiple values
        Task<IEnumerable<FIELD_TYPES>> GetFieldTypesWithMultipleValuesAsync();

        // Get field types for dropdown (simplified for forms)
        Task<IEnumerable<FIELD_TYPES>> GetFieldTypesForDropdownAsync();

        // Get field types with their usage count
        Task<IEnumerable<dynamic>> GetFieldTypesWithUsageCountAsync();

        // -----------------------------
        // Get by Id (newly added)
        // -----------------------------
        Task<FIELD_TYPES> GetByIdAsync(int id, params System.Linq.Expressions.Expression<System.Func<FIELD_TYPES, object>>[] includes);
    }
}
