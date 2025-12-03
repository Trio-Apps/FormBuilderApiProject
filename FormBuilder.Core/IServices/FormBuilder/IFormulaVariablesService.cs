using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Services
{
    public interface IFormulaVariablesService
    {
        #region Basic CRUD Operations
        Task<ApiResponse> GetAllByFormulaIdAsync(int formulaId);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetByIdWithDetailsAsync(int id);
        Task<ApiResponse> CreateAsync(CreateFormulaVariableDto createDto);
        Task<ApiResponse> CreateBulkAsync(BulkCreateFormulaVariablesDto bulkDto);
        Task<ApiResponse> UpdateAsync(int id, UpdateFormulaVariableDto updateDto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> DeleteByFormulaIdAsync(int formulaId);
        #endregion

        #region Query Operations
        Task<ApiResponse> GetBySourceFieldIdAsync(int sourceFieldId);
        Task<ApiResponse> GetActiveByFormulaIdAsync(int formulaId);
        Task<ApiResponse> GetInactiveByFormulaIdAsync(int formulaId);
        Task<ApiResponse> SearchVariablesAsync(int formulaId, string searchTerm);
        Task<ApiResponse> GetByVariableNameAsync(string variableName);
        Task<ApiResponse> GetByFieldTypeAsync(string fieldType, int formulaId);
        Task<ApiResponse> GetVariablesWithoutSourceFieldAsync(int formulaId);
        Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId);
        Task<ApiResponse> CountByFormulaIdAsync(int formulaId);
        Task<ApiResponse> CountActiveByFormulaIdAsync(int formulaId);
        Task<ApiResponse> CountInactiveByFormulaIdAsync(int formulaId);
        #endregion

        #region Validation Operations
        Task<ApiResponse> VariableNameExistsAsync(string variableName, int formulaId, int? excludeId = null);
        Task<ApiResponse> IsSourceFieldUsedAsync(int fieldId);
        Task<ApiResponse> IsSourceFieldUsedInOtherFormulasAsync(int fieldId, int excludeFormulaId);
        Task<ApiResponse> ValidateVariableAsync(CreateFormulaVariableDto createDto);
        Task<ApiResponse> IsVariableValidForFormulaAsync(int formulaId, string variableName, int sourceFieldId);
        #endregion

        #region Utility Operations
        Task<ApiResponse> GetFormulaVariablesWithDetailsAsync(int formulaId);
        Task<ApiResponse> HasFormulaVariablesAsync(int formulaId);
        Task<ApiResponse> HasActiveVariablesAsync(int formulaId);
        Task<ApiResponse> UpdateVariableNameAsync(int id, string variableName);
        Task<ApiResponse> UpdateSourceFieldAsync(int id, int sourceFieldId);
        Task<ApiResponse> GetVariableNamesInFormulaAsync(int formulaId);
        Task<ApiResponse> GetVariableMappingsForFormulaAsync(int formulaId);
        Task<ApiResponse> GetVariableCountByFormulasAsync(List<int> formulaIds);
        Task<ApiResponse> GetVariablesWithFormulaDetailsAsync(int formulaId);
        #endregion

        #region Status Management
        Task<ApiResponse> ToggleActiveAsync(int id, bool isActive);
        Task<ApiResponse> BatchToggleActiveAsync(List<int> variableIds, bool isActive);
        Task<ApiResponse> GetStatisticsByFormulaIdAsync(int formulaId);
        #endregion
    }
}