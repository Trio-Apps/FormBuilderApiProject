using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.froms;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Repositories
{
    public interface IFormulaVariablesRepository : IBaseRepository<FORMULA_VARIABLES>
    {
        // Override BaseRepository methods with "new" keyword
        new Task<FORMULA_VARIABLES> GetByIdAsync(int id);
        new Task<IEnumerable<FORMULA_VARIABLES>> GetAllAsync();
        new Task<IEnumerable<FORMULA_VARIABLES>> GetAllAsync(Expression<Func<FORMULA_VARIABLES, bool>>? filter = null, params Expression<Func<FORMULA_VARIABLES, object>>[] includes);
        new Task<FORMULA_VARIABLES> SingleOrDefaultAsync(Expression<Func<FORMULA_VARIABLES, bool>> filter, bool asNoTracking = false, params Expression<Func<FORMULA_VARIABLES, object>>[] includes);
        new Task<int> CountAsync(Expression<Func<FORMULA_VARIABLES, bool>>? filter = null);
        new Task<bool> AnyAsync(Expression<Func<FORMULA_VARIABLES, bool>>? filter = null);

        // Custom queries for FORMULA_VARIABLES entity
        Task<FORMULA_VARIABLES> GetByVariableNameAsync(string variableName, int formulaId, params Expression<Func<FORMULA_VARIABLES, object>>[] includes);
        Task<IEnumerable<FORMULA_VARIABLES>> GetByFormulaIdAsync(int formulaId, params Expression<Func<FORMULA_VARIABLES, object>>[] includes);
        Task<IEnumerable<FORMULA_VARIABLES>> GetActiveByFormulaIdAsync(int formulaId, params Expression<Func<FORMULA_VARIABLES, object>>[] includes);
        Task<IEnumerable<FORMULA_VARIABLES>> GetBySourceFieldIdAsync(int sourceFieldId, params Expression<Func<FORMULA_VARIABLES, object>>[] includes);
        Task<IEnumerable<FORMULA_VARIABLES>> GetVariablesWithoutSourceFieldAsync(int formulaId, params Expression<Func<FORMULA_VARIABLES, object>>[] includes);
        Task<bool> VariableNameExistsAsync(string variableName, int formulaId, int? excludeId = null);
        Task<bool> HasActiveVariablesAsync(int formulaId);
        Task<IEnumerable<FORMULA_VARIABLES>> GetVariablesWithDetailsAsync(int formulaId = 0);
        Task<FORMULA_VARIABLES> GetByIdWithDetailsAsync(int id);
        Task<bool> IsActiveAsync(int id);

        // Validation methods
        Task<bool> IsVariableValidForFormulaAsync(int formulaId, string variableName, int sourceFieldId);
        Task<IEnumerable<string>> GetVariableNamesInFormulaAsync(int formulaId);

        // Field relationship helper methods
        Task<int?> GetFormulaIdFromVariableIdAsync(int variableId);
        Task<int?> GetFormulaIdFromVariableNameAsync(string variableName, int formBuilderId);
        Task<bool> VariableBelongsToFormulaAsync(int variableId, int formulaId);
        Task<bool> VariableNameBelongsToFormulaAsync(string variableName, int formulaId);
        Task<IEnumerable<FORM_FIELDS>> GetSourceFieldsByFormulaIdAsync(int formulaId);

        // Statistics and counts
        Task<IEnumerable<FORMULA_VARIABLES>> GetFormulaVariablesWithDetailsAsync(int formulaId);
        Task<int> CountFormulaVariablesAsync(int formulaId);
        Task<bool> HasFormulaVariablesAsync(int formulaId);
        Task<int> CountActiveByFormulaIdAsync(int formulaId);
        Task<int> CountInactiveByFormulaIdAsync(int formulaId);

        // Search and filter methods
        Task<IEnumerable<FORMULA_VARIABLES>> SearchVariablesAsync(int formulaId, string searchTerm);
        Task<IEnumerable<FORMULA_VARIABLES>> GetByFieldTypeAsync(string fieldType, int formulaId);
        Task<IEnumerable<FORMULA_VARIABLES>> GetByFormBuilderIdAsync(int formBuilderId);
        Task<IEnumerable<FORMULA_VARIABLES>> GetInactiveByFormulaIdAsync(int formulaId);

        // Bulk operations
        Task<Dictionary<int, int>> GetVariableCountByFormulaAsync(List<int> formulaIds);
        Task<IEnumerable<FORMULA_VARIABLES>> GetVariablesWithFormulaDetailsAsync(int formulaId);

        // Source field validation
        Task<bool> IsSourceFieldUsedAsync(int fieldId);
        Task<bool> IsSourceFieldUsedInOtherFormulasAsync(int fieldId, int excludeFormulaId);

        // Variable mapping helper
        Task<Dictionary<string, FORM_FIELDS>> GetVariableMappingsForFormulaAsync(int formulaId);
    }
}