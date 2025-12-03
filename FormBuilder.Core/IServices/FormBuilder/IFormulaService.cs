using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Services
{
    public interface IFormulaService
    {
        #region Basic CRUD Operations
        Task<ApiResponse> GetAllAsync(int formBuilderId);
        Task<ApiResponse> GetByIdAsync(int id);
        Task<ApiResponse> GetByCodeAsync(string code, int formBuilderId);
        Task<ApiResponse> CreateAsync(CreateFormulaDto createDto);
        Task<ApiResponse> UpdateAsync(int id, UpdateFormulaDto updateDto);
        Task<ApiResponse> DeleteAsync(int id);
        Task<ApiResponse> DeleteByFormBuilderIdAsync(int formBuilderId);
        #endregion

        #region Status Management
        Task<ApiResponse> ToggleActiveAsync(int id, bool isActive);
        Task<ApiResponse> GetActiveAsync(int formBuilderId);
        Task<ApiResponse> GetInactiveAsync(int formBuilderId);
        #endregion

        #region Query Operations
        Task<ApiResponse> GetByFormBuilderAsync(int formBuilderId);
        Task<ApiResponse> GetByResultFieldAsync(int? resultFieldId);
        Task<ApiResponse> GetFormulasWithoutResultFieldAsync(int formBuilderId);
        Task<ApiResponse> GetFormulasWithDetailsAsync(int formBuilderId = 0);
        Task<ApiResponse> GetByIdWithDetailsAsync(int id);
        Task<ApiResponse> SearchFormulasAsync(string searchTerm, int formBuilderId);
        #endregion

        #region Validation Operations
        Task<ApiResponse> ValidateExpressionAsync(ValidateExpressionDto validationDto);
        Task<ApiResponse> CodeExistsAsync(string code, int formBuilderId, int? excludeId = null);
        Task<ApiResponse> IsActiveAsync(int id);
        Task<ApiResponse> HasActiveFormulasAsync(int formBuilderId);
        Task<ApiResponse> IsExpressionValidForFormAsync(string expressionText, int formBuilderId);
        #endregion

        #region Utility Operations
        Task<ApiResponse> GetReferencedFieldCodesAsync(int formulaId);
        Task<ApiResponse> CountFormulasAsync(int formBuilderId);
        Task<ApiResponse> UpdateFormulaExpressionAsync(int id, string expressionText);
        Task<ApiResponse> GetFieldCodesForFormAsync(int formBuilderId);
        Task<ApiResponse> ValidateExpressionWithDetailsAsync(ValidateExpressionDto validationDto);
        #endregion

        #region FORMULA_VARIABLES Operations
        Task<ApiResponse> GetFormulaVariablesAsync(int formulaId);
        Task<ApiResponse> CountVariablesAsync(int formulaId);
        Task<ApiResponse> HasVariablesAsync(int formulaId);
        #endregion

        #region Additional Formula Operations
        Task<ApiResponse> GetFormulasByFieldAsync(int fieldId);
        Task<ApiResponse> GetFormulaStatisticsAsync(int formBuilderId);
        Task<ApiResponse> BatchUpdateFormulaStatusAsync(List<int> formulaIds, bool isActive);
        Task<ApiResponse> DuplicateFormulaAsync(int sourceFormulaId, DuplicateFormulaDto duplicateDto);
        #endregion

        #region Formula Calculation Operations
        Task<ApiResponse> CalculateFormulaAsync(int formulaId, Dictionary<string, object> fieldValues = null);
        Task<ApiResponse> CalculateExpressionAsync(string expressionText, Dictionary<string, object> fieldValues);
        Task<ApiResponse> BatchCalculateFormulasAsync(int formBuilderId, Dictionary<string, object> fieldValues);
        Task<ApiResponse> PreviewCalculationAsync(PreviewCalculationDto previewDto);
        Task<ApiResponse> TestFormulaWithSampleDataAsync(int formulaId);
        Task<ApiResponse> CalculateWithAllOperationsAsync(string expressionText, Dictionary<string, object> fieldValues);
        Task<ApiResponse> SafeCalculateExpressionAsync(string expressionText, Dictionary<string, object> fieldValues);
        #endregion
    }
}