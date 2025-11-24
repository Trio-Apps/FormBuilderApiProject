using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public interface IFormFieldService
    {
        // Basic CRUD Operations
        Task<FORM_FIELDS> CreateFieldAsync(FORM_FIELDS fieldEntity);
        Task<FORM_FIELDS?> GetFieldByIdAsync(int id, bool asNoTracking = false);
        Task<IEnumerable<FORM_FIELDS>> GetFieldsByTabIdAsync(int tabId);
        Task<bool> UpdateFieldAsync(FORM_FIELDS fieldEntity);
        Task<bool> DeleteFieldAsync(int id);

        // Validation Operations
        Task<bool> IsFieldCodeUniqueAsync(string fieldCode, int? ignoreId = null);
        Task<bool> IsFieldNameUniqueAsync(string fieldName, int? ignoreId = null, int? tabId = null);

        // Additional Operations
        Task<FORM_FIELDS?> GetFieldWithDetailsAsync(int id, bool asNoTracking = false);
        Task<IEnumerable<FORM_FIELDS>> GetAllFieldsAsync(Expression<Func<FORM_FIELDS, bool>>? filter = null);
        Task<bool> FieldExistsAsync(int id);
        Task<IEnumerable<FORM_FIELDS>> GetActiveFieldsAsync();
        Task<IEnumerable<FORM_FIELDS>> GetFieldsByFormIdAsync(int formBuilderId);
        Task<int> GetFieldsCountAsync(int tabId);
        Task<bool> SoftDeleteFieldAsync(int id);

        // Specialized Queries
        Task<IEnumerable<FORM_FIELDS>> GetMandatoryFieldsAsync(int tabId);
        Task<IEnumerable<FORM_FIELDS>> GetVisibleFieldsAsync(int tabId);
        Task<IEnumerable<FORM_FIELDS>> GetFieldsByDataTypeAsync(string dataType);
        Task<IEnumerable<FORM_FIELDS>> GetFieldsByFieldTypeAsync(int fieldTypeId);

        // Bulk Operations
        Task<bool> UpdateFieldsOrderAsync(Dictionary<int, int> fieldOrders);
        Task<IEnumerable<FORM_FIELDS>> GetFieldsWithValidationRulesAsync(int tabId);
    }
}