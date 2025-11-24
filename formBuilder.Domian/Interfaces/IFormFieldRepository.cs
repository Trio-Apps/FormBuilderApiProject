using formBuilder.Domian.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces
{
    public interface IFormFieldRepository : IBaseRepository<FORM_FIELDS>
    {
        // Field Code Validation
        Task<bool> IsFieldCodeUniqueAsync(string fieldCode, int? ignoreId = null);
        Task<bool> IsFieldNameUniqueAsync(string fieldName, int? ignoreId = null, int? tabId = null);

        // Specialized Queries
        Task<IEnumerable<FORM_FIELDS>> GetFieldsByTabIdAsync(int tabId);
        Task<IEnumerable<FORM_FIELDS>> GetFieldsByFormIdAsync(int formBuilderId);
        Task<IEnumerable<FORM_FIELDS>> GetMandatoryFieldsAsync(int tabId);
        Task<IEnumerable<FORM_FIELDS>> GetVisibleFieldsAsync(int tabId);
        Task<IEnumerable<FORM_FIELDS>> GetFieldsByDataTypeAsync(string dataType);
        Task<IEnumerable<FORM_FIELDS>> GetFieldsByFieldTypeAsync(int fieldTypeId);

        // Count Operations
        Task<int> GetFieldsCountByTabAsync(int tabId);
        Task<int> GetFieldsCountByFormAsync(int formBuilderId);

        // Bulk Operations
        Task<bool> UpdateFieldOrderAsync(int fieldId, int newOrder);
        Task<bool> UpdateFieldsOrderAsync(Dictionary<int, int> fieldOrders);

        // Validation Rules
        Task<IEnumerable<FORM_FIELDS>> GetFieldsWithValidationRulesAsync(int tabId);
    }
}