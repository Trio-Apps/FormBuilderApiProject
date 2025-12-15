using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Application.DTOs.Formula;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormulaService : IFormulaService
    {
        private readonly IunitOfwork _unitOfWork;
        private readonly IFormulasRepository _formulasRepository;

        public FormulaService(IunitOfwork unitOfWork, IFormulasRepository formulasRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _formulasRepository = formulasRepository ?? throw new ArgumentNullException(nameof(formulasRepository));
        }

        #region Basic CRUD Operations
        public async Task<ApiResponse> GetAllAsync(int formBuilderId)
        {
            try
            {
                var formulas = await _formulasRepository.GetByFormBuilderAsync(formBuilderId);
                var formulaDtos = formulas.Select(ToDto).ToList();
                return new ApiResponse(200, "Formulas retrieved successfully", formulaDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formulas: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var formula = await _formulasRepository.GetByIdAsync(id);
                if (formula == null)
                    return new ApiResponse(404, "Formula not found");

                var formulaDto = ToDto(formula);
                return new ApiResponse(200, "Formula retrieved successfully", formulaDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formula: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByCodeAsync(string code, int formBuilderId)
        {
            try
            {
                var formula = await _formulasRepository.GetByCodeAsync(code, formBuilderId);
                if (formula == null)
                    return new ApiResponse(404, "Formula not found");

                var formulaDto = ToDto(formula);
                return new ApiResponse(200, "Formula retrieved successfully", formulaDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formula: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateFormulaDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Validate form builder exists
                var formBuilderExists = await _unitOfWork.Repositary<FORM_BUILDER>()
                    .AnyAsync(fb => fb.Id == createDto.FormBuilderId);

                if (!formBuilderExists)
                    return new ApiResponse(404, "Form builder not found");

                // Check if code already exists
                var codeExists = await _formulasRepository.CodeExistsAsync(createDto.Code, createDto.FormBuilderId);
                if (codeExists)
                    return new ApiResponse(400, "Formula code already exists for this form");

                // Validate result field if provided
                if (createDto.ResultFieldId.HasValue)
                {
                    var fieldBelongsToForm = await _formulasRepository.FieldBelongsToFormBuilderAsync(
                        createDto.ResultFieldId.Value,
                        createDto.FormBuilderId);

                    if (!fieldBelongsToForm)
                        return new ApiResponse(400, "Result field not found or doesn't belong to the form");
                }

                // Validate expression
                var validationResult = await ValidateExpressionAsync(new ValidateExpressionDto
                {
                    ExpressionText = createDto.ExpressionText,
                    FormBuilderId = createDto.FormBuilderId
                });

                if (validationResult.StatusCode != 200)
                    return validationResult;

                // Create formula entity
                var entity = new FORMULAS
                {
                    FormBuilderId = createDto.FormBuilderId,
                    Name = createDto.Name,
                    Code = createDto.Code,
                    ExpressionText = createDto.ExpressionText,
                    ResultFieldId = createDto.ResultFieldId,
                    IsActive = createDto.IsActive,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _unitOfWork.FormulasRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                var createdFormula = await _formulasRepository.GetByIdWithDetailsAsync(entity.Id);
                return new ApiResponse(200, "Formula created successfully", ToDto(createdFormula));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating formula: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormulaDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _formulasRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Formula not found");

                // Check if code already exists (excluding current record)
                if (!string.IsNullOrEmpty(updateDto.Code) && updateDto.Code != entity.Code)
                {
                    var codeExists = await _formulasRepository.CodeExistsAsync(
                        updateDto.Code, entity.FormBuilderId, id);

                    if (codeExists)
                        return new ApiResponse(400, "Formula code already exists for this form");
                }

                // Validate result field if provided
                if (updateDto.ResultFieldId.HasValue)
                {
                    var fieldBelongsToForm = await _formulasRepository.FieldBelongsToFormBuilderAsync(
                        updateDto.ResultFieldId.Value,
                        entity.FormBuilderId);

                    if (!fieldBelongsToForm)
                        return new ApiResponse(400, "Result field not found or doesn't belong to the form");
                }

                // Validate expression if provided
                if (!string.IsNullOrEmpty(updateDto.ExpressionText))
                {
                    var validationResult = await ValidateExpressionAsync(new ValidateExpressionDto
                    {
                        ExpressionText = updateDto.ExpressionText,
                        FormBuilderId = entity.FormBuilderId
                    });

                    if (validationResult.StatusCode != 200)
                        return validationResult;
                }

                // Update entity
                MapUpdate(updateDto, entity);
                entity.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.FormulasRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedFormula = await _formulasRepository.GetByIdWithDetailsAsync(id);
                return new ApiResponse(200, "Formula updated successfully", ToDto(updatedFormula));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating formula: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _formulasRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Formula not found");

                // Check if formula has variables
                var hasVariables = await _formulasRepository.HasFormulaVariablesAsync(id);
                if (hasVariables)
                {
                    return new ApiResponse(400, "Cannot delete formula because it has associated variables. Delete variables first.");
                }

                _unitOfWork.FormulasRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Formula deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting formula: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var formulas = await _formulasRepository.GetByFormBuilderAsync(formBuilderId);
                if (!formulas.Any())
                    return new ApiResponse(404, "No formulas found for this form builder");

                // Check if any formula has variables
                var formulasWithVariables = new List<FORMULAS>();
                foreach (var formula in formulas)
                {
                    var hasVariables = await _formulasRepository.HasFormulaVariablesAsync(formula.Id);
                    if (hasVariables)
                    {
                        formulasWithVariables.Add(formula);
                    }
                }

                if (formulasWithVariables.Any())
                {
                    return new ApiResponse(400,
                        $"Cannot delete {formulasWithVariables.Count} formulas because they have associated variables. Delete variables first.");
                }

                var formulasList = formulas.ToList();
                _unitOfWork.FormulasRepository.DeleteRange(formulasList);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, $"Deleted {formulasList.Count} formulas successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting formulas: {ex.Message}");
            }
        }
        #endregion

        #region Status Management
        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _formulasRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Formula not found");

                entity.IsActive = isActive;
                entity.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.FormulasRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Formula {message} successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling formula active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveAsync(int formBuilderId)
        {
            try
            {
                var formulas = await _formulasRepository.GetActiveByFormBuilderAsync(formBuilderId);
                var formulaDtos = formulas.Select(ToDto).ToList();
                return new ApiResponse(200, "Active formulas retrieved successfully", formulaDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active formulas: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetInactiveAsync(int formBuilderId)
        {
            try
            {
                var formulas = await _formulasRepository.GetAllAsync(f => f.FormBuilderId == formBuilderId && !f.IsActive);
                var formulaDtos = formulas.Select(ToDto).ToList();
                return new ApiResponse(200, "Inactive formulas retrieved successfully", formulaDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving inactive formulas: {ex.Message}");
            }
        }
        #endregion

        #region Query Operations
        public async Task<ApiResponse> GetByFormBuilderAsync(int formBuilderId)
        {
            return await GetAllAsync(formBuilderId);
        }

        public async Task<ApiResponse> GetByResultFieldAsync(int? resultFieldId)
        {
            try
            {
                var formulas = await _formulasRepository.GetByResultFieldAsync(resultFieldId);
                var formulaDtos = formulas.Select(ToDto).ToList();
                return new ApiResponse(200, "Formulas retrieved successfully", formulaDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formulas: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetFormulasWithoutResultFieldAsync(int formBuilderId)
        {
            try
            {
                var formulas = await _formulasRepository.GetFormulasWithoutResultFieldAsync(formBuilderId);
                var formulaDtos = formulas.Select(ToDto).ToList();
                return new ApiResponse(200, "Formulas without result field retrieved successfully", formulaDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formulas: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetFormulasWithDetailsAsync(int formBuilderId = 0)
        {
            try
            {
                var formulas = await _formulasRepository.GetFormulasWithDetailsAsync(formBuilderId);
                var formulaDtos = formulas.Select(ToDto).ToList();
                return new ApiResponse(200, "Formulas with details retrieved successfully", formulaDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formulas with details: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdWithDetailsAsync(int id)
        {
            try
            {
                var formula = await _formulasRepository.GetByIdWithDetailsAsync(id);
                if (formula == null)
                    return new ApiResponse(404, "Formula not found");

                var formulaDto = ToDto(formula);

                // Get formula variables
                var variables = await _formulasRepository.GetFormulaVariablesWithDetailsAsync(id);
                formulaDto.VariableCount = variables.Count();

                // Use the fully qualified namespace
                formulaDto.Variables = variables.Select(v => new FormulaVariableDto
                {
                    Id = v.Id,
                    FormulaId = v.FormulaId,
                    SourceFieldId = v.FORM_FIELDS.Id,
                    Formulaname = v.FORM_FIELDS?.FieldName,
                    SourceFieldName = v.FORM_FIELDS?.FIELD_TYPES?.TypeName,
                    VariableName = v.VariableName
                }).ToList();

                return new ApiResponse(200, "Formula with details retrieved successfully", formulaDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formula with details: {ex.Message}");
            }
        }

        public async Task<ApiResponse> SearchFormulasAsync(string searchTerm, int formBuilderId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(searchTerm))
                    return new ApiResponse(400, "Search term is required");

                var formulas = await _formulasRepository.GetAllAsync(
                    f => f.FormBuilderId == formBuilderId &&
                        (f.Name.Contains(searchTerm) ||
                         f.Code.Contains(searchTerm) ||
                         f.ExpressionText.Contains(searchTerm))
                );

                var formulaDtos = formulas.Select(ToDto).ToList();
                return new ApiResponse(200, "Formulas search completed successfully", formulaDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error searching formulas: {ex.Message}");
            }
        }
        #endregion

        #region Validation Operations
        public async Task<ApiResponse> ValidateExpressionAsync(ValidateExpressionDto validationDto)
        {
            try
            {
                if (validationDto == null)
                    return new ApiResponse(400, "Validation DTO is required");

                if (string.IsNullOrWhiteSpace(validationDto.ExpressionText))
                    return new ApiResponse(400, "Expression text is required", false);

                var fieldCodes = await _formulasRepository.GetReferencedFieldCodesInExpressionAsync(
                    validationDto.ExpressionText);

                if (!fieldCodes.Any())
                {
                    var isMathValid = await ValidateMathematicalExpressionAsync(validationDto.ExpressionText);
                    if (!isMathValid)
                        return new ApiResponse(400, "Invalid mathematical expression", false);

                    return new ApiResponse(200, "Mathematical expression is valid", true);
                }

                var invalidFieldCodes = new List<string>();

                foreach (var fieldCode in fieldCodes)
                {
                    var fieldExists = await _unitOfWork.Repositary<FORM_FIELDS>()
                        .AnyAsync(f => f.FieldCode == fieldCode &&
                                     f.FORM_TABS.FormBuilderId == validationDto.FormBuilderId &&
                                     f.IsActive);

                    if (!fieldExists)
                    {
                        invalidFieldCodes.Add(fieldCode);
                    }
                }

                if (invalidFieldCodes.Any())
                {
                    return new ApiResponse(400,
                        $"Expression contains invalid field codes: {string.Join(", ", invalidFieldCodes)}",
                        false);
                }

                return new ApiResponse(200, "Expression is valid", true);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error validating expression: {ex.Message}", false);
            }
        }

        private async Task<bool> ValidateMathematicalExpressionAsync(string expressionText)
        {
            try
            {
                var cleanExpression = expressionText?.Replace(" ", "");

                if (string.IsNullOrEmpty(cleanExpression))
                    return false;

                var processedExpression = PreprocessExpression(cleanExpression);

                var validPattern = @"^[\d\s\+\-\*\/\(\)\.\^%a-zA-Z_]+$";

                if (!Regex.IsMatch(processedExpression, validPattern))
                    return false;

                if (ContainsDivisionByZero(processedExpression))
                    return false;

                if (ContainsModulusByZero(processedExpression))
                    return false;

                if (ContainsInvalidOperations(processedExpression))
                    return false;

                try
                {
                    var result = AdvancedEvaluate(processedExpression);

                    return result != null && !double.IsNaN(Convert.ToDouble(result)) &&
                           !double.IsInfinity(Convert.ToDouble(result));
                }
                catch (DivideByZeroException)
                {
                    return false;
                }
                catch (ArgumentException)
                {
                    return false;
                }
                catch
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<ApiResponse> CodeExistsAsync(string code, int formBuilderId, int? excludeId = null)
        {
            try
            {
                var exists = await _formulasRepository.CodeExistsAsync(code, formBuilderId, excludeId);
                return new ApiResponse(200, "Code existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking code existence: {ex.Message}");
            }
        }

        public async Task<ApiResponse> IsActiveAsync(int id)
        {
            try
            {
                var isActive = await _formulasRepository.IsActiveAsync(id);
                return new ApiResponse(200, "Formula active status checked successfully", isActive);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking formula active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> HasActiveFormulasAsync(int formBuilderId)
        {
            try
            {
                var hasActive = await _formulasRepository.HasActiveFormulasAsync(formBuilderId);
                return new ApiResponse(200, "Active formulas check completed", hasActive);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking active formulas: {ex.Message}");
            }
        }

        public async Task<ApiResponse> IsExpressionValidForFormAsync(string expressionText, int formBuilderId)
        {
            try
            {
                var validationResult = await ValidateExpressionAsync(new ValidateExpressionDto
                {
                    ExpressionText = expressionText,
                    FormBuilderId = formBuilderId
                });

                return new ApiResponse(validationResult.StatusCode, validationResult.Message, validationResult.StatusCode == 200);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error validating expression: {ex.Message}", false);
            }
        }
        #endregion

        #region Utility Operations
        public async Task<ApiResponse> GetReferencedFieldCodesAsync(int formulaId)
        {
            try
            {
                var formula = await _formulasRepository.GetByIdAsync(formulaId);
                if (formula == null)
                    return new ApiResponse(404, "Formula not found");

                var fieldCodes = await _formulasRepository.GetReferencedFieldCodesInExpressionAsync(formula.ExpressionText);
                return new ApiResponse(200, "Referenced field codes retrieved successfully", fieldCodes);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving referenced field codes: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CountFormulasAsync(int formBuilderId)
        {
            try
            {
                var allCount = await _formulasRepository.CountAsync(f => f.FormBuilderId == formBuilderId);
                var activeCount = await _formulasRepository.CountAsync(f => f.FormBuilderId == formBuilderId && f.IsActive);
                var inactiveCount = allCount - activeCount;

                var result = new
                {
                    TotalCount = allCount,
                    ActiveCount = activeCount,
                    InactiveCount = inactiveCount
                };

                return new ApiResponse(200, "Formulas count retrieved successfully", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error counting formulas: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateFormulaExpressionAsync(int id, string expressionText)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expressionText))
                    return new ApiResponse(400, "Expression text is required");

                var formula = await _formulasRepository.GetByIdAsync(id);
                if (formula == null)
                    return new ApiResponse(404, "Formula not found");

                var validationResult = await ValidateExpressionAsync(new ValidateExpressionDto
                {
                    ExpressionText = expressionText,
                    FormBuilderId = formula.FormBuilderId
                });

                if (validationResult.StatusCode != 200)
                    return validationResult;

                formula.ExpressionText = expressionText;
                formula.UpdatedDate = DateTime.UtcNow;

                _unitOfWork.FormulasRepository.Update(formula);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Formula expression updated successfully", ToDto(formula));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating formula expression: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetFieldCodesForFormAsync(int formBuilderId)
        {
            try
            {
                var fields = await _formulasRepository.GetFieldsByFormBuilderAsync(formBuilderId);

                var fieldInfo = fields.Select(f => new FormulaFieldInfoDto
                {
                    FieldId = f.Id,
                    FieldCode = f.FieldCode,
                    FieldName = f.FieldName,
                    FieldType = f.FIELD_TYPES?.TypeName,
                    TabName = f.FORM_TABS?.TabName,
                    FormBuilderId = formBuilderId,
                    IsActive = f.IsActive
                }).ToList();

                return new ApiResponse(200, "Field codes retrieved successfully", fieldInfo);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving field codes: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ValidateExpressionWithDetailsAsync(ValidateExpressionDto validationDto)
        {
            try
            {
                if (validationDto == null)
                    return new ApiResponse(400, "Validation DTO is required");

                var fieldCodes = await _formulasRepository.GetReferencedFieldCodesInExpressionAsync(
                    validationDto.ExpressionText);

                var result = new ValidateExpressionResultDto();

                if (!fieldCodes.Any())
                {
                    var isMathValid = await ValidateMathematicalExpressionAsync(validationDto.ExpressionText);
                    result.IsValid = isMathValid;

                    if (isMathValid)
                    {
                        result.ValidFieldCodes.Add("MATHEMATICAL_EXPRESSION");
                        result.FieldDetails.Add(new FormulaFieldInfoDto
                        {
                            FieldId = 0,
                            FieldCode = "MATHEMATICAL_EXPRESSION",
                            FieldName = "Mathematical Expression",
                            FieldType = "Number",
                            TabName = "N/A",
                            FormBuilderId = validationDto.FormBuilderId,
                            FormBuilderName = "Mathematical Operation",
                            IsActive = true
                        });
                    }
                    else
                    {
                        result.InvalidFieldCodes.Add("INVALID_MATH");
                    }

                    return new ApiResponse(200, "Expression validation completed", result);
                }

                foreach (var fieldCode in fieldCodes)
                {
                    var field = await _unitOfWork.Repositary<FORM_FIELDS>()
                        .SingleOrDefaultAsync(f => f.FieldCode == fieldCode &&
                                                 f.FORM_TABS.FormBuilderId == validationDto.FormBuilderId &&
                                                 f.IsActive,
                            includes: new Expression<Func<FORM_FIELDS, object>>[] {
                                f => f.FORM_TABS,
                                f => f.FORM_TABS.FORM_BUILDER,
                                f => f.FIELD_TYPES
                            });

                    if (field != null)
                    {
                        result.ValidFieldCodes.Add(fieldCode);
                        result.FieldDetails.Add(new FormulaFieldInfoDto
                        {
                            FieldId = field.Id,
                            FieldCode = field.FieldCode,
                            FieldName = field.FieldName,
                            FieldType = field.FIELD_TYPES?.TypeName,
                            TabName = field.FORM_TABS?.TabName,
                            FormBuilderId = validationDto.FormBuilderId,
                            FormBuilderName = field.FORM_TABS?.FORM_BUILDER?.FormName,
                            IsActive = field.IsActive
                        });
                    }
                    else
                    {
                        result.InvalidFieldCodes.Add(fieldCode);
                    }
                }

                result.IsValid = !result.InvalidFieldCodes.Any();

                return new ApiResponse(200, "Expression validation completed", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error validating expression: {ex.Message}");
            }
        }
        #endregion

        #region FORMULA_VARIABLES Operations
        public async Task<ApiResponse> GetFormulaVariablesAsync(int formulaId)
        {
            try
            {
                var variables = await _formulasRepository.GetFormulaVariablesWithDetailsAsync(formulaId);

                var variableDtos = variables.Select(v => new FormulaVariableDto
                {
                    Id = v.Id,
                    FormulaId = v.FormulaId,
                    SourceFieldId = v.FORM_FIELDS.Id,
                    SourceFieldName = v.FORM_FIELDS?.FieldName,
                    VariableName = v.VariableName
                }).ToList();

                return new ApiResponse(200, "Formula variables retrieved successfully", variableDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formula variables: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CountVariablesAsync(int formulaId)
        {
            try
            {
                var count = await _formulasRepository.CountFormulaVariablesAsync(formulaId);
                return new ApiResponse(200, "Variables count retrieved successfully", count);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error counting variables: {ex.Message}");
            }
        }

        public async Task<ApiResponse> HasVariablesAsync(int formulaId)
        {
            try
            {
                var hasVariables = await _formulasRepository.HasFormulaVariablesAsync(formulaId);
                return new ApiResponse(200, "Variables existence checked successfully", hasVariables);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking variables existence: {ex.Message}");
            }
        }
        #endregion

        #region Additional Formula Operations
        public async Task<ApiResponse> GetFormulasByFieldAsync(int fieldId)
        {
            try
            {
                var formId = await _formulasRepository.GetFormIdFromFieldIdAsync(fieldId);
                if (!formId.HasValue)
                    return new ApiResponse(404, "Field not found or doesn't belong to any form");

                var formulas = await _formulasRepository.GetAllAsync();
                var formulasWithField = new List<FormulaDto>();

                foreach (var formula in formulas)
                {
                    var fieldCodes = await _formulasRepository.GetReferencedFieldCodesInExpressionAsync(formula.ExpressionText);
                    var field = await _unitOfWork.Repositary<FORM_FIELDS>().SingleOrDefaultAsync(s => s.Id == fieldId);

                    if (field != null && fieldCodes.Contains(field.FieldCode))
                    {
                        formulasWithField.Add(ToDto(formula));
                    }
                }

                return new ApiResponse(200, "Formulas by field retrieved successfully", formulasWithField);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formulas by field: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetFormulaStatisticsAsync(int formBuilderId)
        {
            try
            {
                var allFormulas = await _formulasRepository.GetByFormBuilderAsync(formBuilderId);
                var activeFormulas = allFormulas.Where(f => f.IsActive).ToList();
                var inactiveFormulas = allFormulas.Where(f => !f.IsActive).ToList();

                var formulasWithResultField = allFormulas.Where(f => f.ResultFieldId.HasValue).ToList();
                var formulasWithoutResultField = allFormulas.Where(f => !f.ResultFieldId.HasValue).ToList();

                var totalVariables = 0;
                foreach (var formula in allFormulas)
                {
                    totalVariables += await _formulasRepository.CountFormulaVariablesAsync(formula.Id);
                }

                var statistics = new
                {
                    TotalFormulas = allFormulas.Count(),
                    ActiveFormulas = activeFormulas.Count,
                    InactiveFormulas = inactiveFormulas.Count,
                    FormulasWithResultField = formulasWithResultField.Count,
                    FormulasWithoutResultField = formulasWithoutResultField.Count,
                    TotalVariables = totalVariables,
                    AverageVariablesPerFormula = allFormulas.Any() ? (double)totalVariables / allFormulas.Count() : 0
                };

                return new ApiResponse(200, "Formula statistics retrieved successfully", statistics);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving formula statistics: {ex.Message}");
            }
        }

        public async Task<ApiResponse> BatchUpdateFormulaStatusAsync(List<int> formulaIds, bool isActive)
        {
            try
            {
                if (formulaIds == null || !formulaIds.Any())
                    return new ApiResponse(400, "Formula IDs are required");

                var updatedCount = 0;
                var failedIds = new List<int>();

                foreach (var formulaId in formulaIds)
                {
                    try
                    {
                        var formula = await _formulasRepository.GetByIdAsync(formulaId);
                        if (formula != null)
                        {
                            formula.IsActive = isActive;
                            formula.UpdatedDate = DateTime.UtcNow;
                            _unitOfWork.FormulasRepository.Update(formula);
                            updatedCount++;
                        }
                        else
                        {
                            failedIds.Add(formulaId);
                        }
                    }
                    catch
                    {
                        failedIds.Add(formulaId);
                    }
                }

                await _unitOfWork.CompleteAsyn();

                var result = new
                {
                    UpdatedCount = updatedCount,
                    FailedCount = failedIds.Count,
                    FailedIds = failedIds,
                    Message = $"Successfully updated {updatedCount} formulas. Failed to update {failedIds.Count} formulas."
                };

                return new ApiResponse(200, "Batch update completed", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error in batch update: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DuplicateFormulaAsync(int sourceFormulaId, DuplicateFormulaDto duplicateDto)
        {
            try
            {
                var sourceFormula = await _formulasRepository.GetByIdWithDetailsAsync(sourceFormulaId);
                if (sourceFormula == null)
                    return new ApiResponse(404, "Source formula not found");

                var formBuilderExists = await _unitOfWork.Repositary<FORM_BUILDER>()
                    .AnyAsync(fb => fb.Id == duplicateDto.TargetFormBuilderId);

                if (!formBuilderExists)
                    return new ApiResponse(404, "Target form builder not found");

                var codeExists = await _formulasRepository.CodeExistsAsync(duplicateDto.NewCode, duplicateDto.TargetFormBuilderId);
                if (codeExists)
                    return new ApiResponse(400, "Formula code already exists in target form builder");

                var newFormula = new FORMULAS
                {
                    FormBuilderId = duplicateDto.TargetFormBuilderId,
                    Name = duplicateDto.NewName,
                    Code = duplicateDto.NewCode,
                    ExpressionText = sourceFormula.ExpressionText,
                    ResultFieldId = null,
                    IsActive = sourceFormula.IsActive,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow
                };

                _unitOfWork.FormulasRepository.Add(newFormula);
                await _unitOfWork.CompleteAsyn();

                var duplicatedFormula = await _formulasRepository.GetByIdWithDetailsAsync(newFormula.Id);
                return new ApiResponse(200, "Formula duplicated successfully", ToDto(duplicatedFormula));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error duplicating formula: {ex.Message}");
            }
        }
        #endregion

        #region Formula Calculation Operations
        public async Task<ApiResponse> CalculateFormulaAsync(int formulaId, Dictionary<string, object> fieldValues = null)
        {
            try
            {
                var formula = await _formulasRepository.GetByIdWithDetailsAsync(formulaId);
                if (formula == null)
                    return new ApiResponse(404, "Formula not found");

                if (!formula.IsActive)
                    return new ApiResponse(400, "Formula is not active");

                var fieldCodes = await _formulasRepository.GetReferencedFieldCodesInExpressionAsync(formula.ExpressionText);
                var values = fieldValues ?? new Dictionary<string, object>();

                if (!fieldValues?.Any() == true && fieldCodes.Any())
                {
                    foreach (var fieldCode in fieldCodes)
                    {
                        var field = await _unitOfWork.Repositary<FORM_FIELDS>()
                            .SingleOrDefaultAsync(f => f.FieldCode == fieldCode &&
                                                     f.FORM_TABS.FormBuilderId == formula.FormBuilderId);

                        if (field != null)
                        {
                            values[fieldCode] = await GetFieldValueAsync(field.Id);
                        }
                    }
                }

                var result = await CalculateExpressionAsync(formula.ExpressionText, values);

                return new ApiResponse(200, "Formula calculated successfully", new
                {
                    FormulaId = formulaId,
                    FormulaName = formula.Name,
                    FormulaCode = formula.Code,
                    Expression = formula.ExpressionText,
                    FieldValues = values,
                    Result = result.Data
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error calculating formula: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CalculateExpressionAsync(string expressionText, Dictionary<string, object> fieldValues)
        {
            return await CalculateWithAllOperationsAsync(expressionText, fieldValues);
        }

        public async Task<ApiResponse> BatchCalculateFormulasAsync(int formBuilderId, Dictionary<string, object> fieldValues)
        {
            try
            {
                var formulas = await _formulasRepository.GetActiveByFormBuilderAsync(formBuilderId);
                if (!formulas.Any())
                    return new ApiResponse(404, "No active formulas found for this form builder");

                var results = new List<object>();
                var errors = new List<string>();

                foreach (var formula in formulas)
                {
                    try
                    {
                        if (formula.ResultFieldId.HasValue)
                        {
                            var calculation = await CalculateExpressionAsync(formula.ExpressionText, fieldValues);

                            if (calculation.StatusCode == 200)
                            {
                                results.Add(new
                                {
                                    FormulaId = formula.Id,
                                    FormulaName = formula.Name,
                                    FormulaCode = formula.Code,
                                    ResultFieldId = formula.ResultFieldId,
                                    ResultFieldCode = formula.RESULT_FIELD?.FieldCode,
                                    Result = calculation.Data,
                                    Expression = formula.ExpressionText
                                });
                            }
                            else
                            {
                                errors.Add($"Formula '{formula.Code}': {calculation.Message}");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Formula '{formula.Code}': {ex.Message}");
                    }
                }

                return new ApiResponse(200, "Batch calculation completed", new
                {
                    TotalFormulas = formulas.Count(),
                    SuccessCount = results.Count,
                    ErrorCount = errors.Count,
                    Results = results,
                    Errors = errors
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error in batch calculation: {ex.Message}");
            }
        }

        public async Task<ApiResponse> PreviewCalculationAsync(PreviewCalculationDto previewDto)
        {
            try
            {
                if (previewDto == null)
                    return new ApiResponse(400, "Preview DTO is required");

                var validationResult = await ValidateExpressionAsync(new ValidateExpressionDto
                {
                    ExpressionText = previewDto.ExpressionText,
                    FormBuilderId = previewDto.FormBuilderId
                });

                if (validationResult.StatusCode != 200)
                    return validationResult;

                var result = await CalculateExpressionAsync(previewDto.ExpressionText, previewDto.FieldValues);

                var fieldCodes = await _formulasRepository.GetReferencedFieldCodesInExpressionAsync(previewDto.ExpressionText);
                var fieldDetails = new List<object>();

                foreach (var fieldCode in fieldCodes)
                {
                    var field = await _unitOfWork.Repositary<FORM_FIELDS>()
                        .SingleOrDefaultAsync(f => f.FieldCode == fieldCode &&
                                                 f.FORM_TABS.FormBuilderId == previewDto.FormBuilderId);

                    if (field != null)
                    {
                        fieldDetails.Add(new
                        {
                            FieldId = field.Id,
                            FieldCode = field.FieldCode,
                            FieldName = field.FieldName,
                            FieldType = field.FIELD_TYPES?.TypeName,
                            ProvidedValue = previewDto.FieldValues.ContainsKey(fieldCode) ?
                                           previewDto.FieldValues[fieldCode] : null,
                            HasValue = previewDto.FieldValues.ContainsKey(fieldCode)
                        });
                    }
                }

                return new ApiResponse(200, "Calculation preview completed", new
                {
                    Expression = previewDto.ExpressionText,
                    FieldValues = previewDto.FieldValues,
                    ReferencedFields = fieldDetails,
                    CalculationResult = result.Data,
                    CalculationSteps = GenerateCalculationSteps(previewDto.ExpressionText, previewDto.FieldValues)
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error in calculation preview: {ex.Message}");
            }
        }

        public async Task<ApiResponse> TestFormulaWithSampleDataAsync(int formulaId)
        {
            try
            {
                var formula = await _formulasRepository.GetByIdWithDetailsAsync(formulaId);
                if (formula == null)
                    return new ApiResponse(404, "Formula not found");

                var fieldCodes = await _formulasRepository.GetReferencedFieldCodesInExpressionAsync(formula.ExpressionText);
                var sampleData = new Dictionary<string, object>();
                var sampleFieldDetails = new List<object>();

                foreach (var fieldCode in fieldCodes)
                {
                    var field = await _unitOfWork.Repositary<FORM_FIELDS>()
                        .SingleOrDefaultAsync(f => f.FieldCode == fieldCode,
                            includes: new Expression<Func<FORM_FIELDS, object>>[] {
                                f => f.FIELD_TYPES
                            });

                    if (field != null)
                    {
                        var sampleValue = GenerateSampleValue(field.FIELD_TYPES?.TypeName);
                        sampleData[fieldCode] = sampleValue;

                        sampleFieldDetails.Add(new
                        {
                            FieldId = field.Id,
                            FieldCode = field.FieldCode,
                            FieldName = field.FieldName,
                            FieldType = field.FIELD_TYPES?.TypeName,
                            SampleValue = sampleValue,
                            SampleValueType = sampleValue?.GetType().Name
                        });
                    }
                }

                var calculationResult = await CalculateExpressionAsync(formula.ExpressionText, sampleData);

                return new ApiResponse(200, "Formula test with sample data completed", new
                {
                    FormulaId = formulaId,
                    FormulaName = formula.Name,
                    FormulaCode = formula.Code,
                    Expression = formula.ExpressionText,
                    SampleData = sampleData,
                    SampleFieldDetails = sampleFieldDetails,
                    CalculationResult = calculationResult.Data,
                    IsValid = calculationResult.StatusCode == 200
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error testing formula: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CalculateWithAllOperationsAsync(string expressionText, Dictionary<string, object> fieldValues)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expressionText))
                    return new ApiResponse(400, "Expression text is required");

                var processedExpression = expressionText;

                foreach (var kvp in fieldValues)
                {
                    var valueStr = kvp.Value?.ToString() ?? "0";

                    processedExpression = processedExpression
                        .Replace($"[{kvp.Key}]", valueStr)
                        .Replace($"{{{kvp.Key}}}", valueStr)
                        .Replace(kvp.Key, valueStr);
                }

                processedExpression = PreprocessExpression(processedExpression);

                var validationResult = await ValidateMathematicalExpressionAsync(processedExpression);
                if (!validationResult)
                    return new ApiResponse(400, "Invalid mathematical expression");

                var result = AdvancedEvaluate(processedExpression);

                var steps = GenerateCalculationSteps(expressionText, processedExpression, fieldValues);

                return new ApiResponse(200, "Calculation completed successfully", new
                {
                    OriginalExpression = expressionText,
                    ProcessedExpression = processedExpression,
                    FieldValues = fieldValues,
                    Result = result,
                    ResultType = result?.GetType().Name,
                    CalculationSteps = steps,
                    OperationsUsed = ExtractOperationsUsed(expressionText)
                });
            }
            catch (DivideByZeroException ex)
            {
                return new ApiResponse(400, "Division by zero error", new
                {
                    Error = ex.Message,
                    IsDivisionByZero = true
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Calculation error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> SafeCalculateExpressionAsync(string expressionText, Dictionary<string, object> fieldValues)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(expressionText))
                    return new ApiResponse(400, "Expression text is required");

                var processedExpression = expressionText;
                var replacements = new Dictionary<string, string>();

                foreach (var kvp in fieldValues)
                {
                    var valueStr = kvp.Value?.ToString() ?? "0";

                    processedExpression = processedExpression
                        .Replace($"[{kvp.Key}]", valueStr)
                        .Replace($"{{{kvp.Key}}}", valueStr)
                        .Replace(kvp.Key, valueStr);

                    replacements[kvp.Key] = valueStr;
                }

                processedExpression = PreprocessExpression(processedExpression);

                if (ContainsDivisionByZero(processedExpression))
                {
                    return new ApiResponse(400, "Expression contains division by zero", new
                    {
                        Expression = expressionText,
                        EvaluatedExpression = processedExpression,
                        Error = "Division by zero detected",
                        IsDivisionByZero = true
                    });
                }

                var result = SafeEvaluateExpression(processedExpression);

                return new ApiResponse(200, "Expression calculated successfully", new
                {
                    Expression = expressionText,
                    FieldValues = fieldValues,
                    EvaluatedExpression = processedExpression,
                    Result = result,
                    IsValid = true
                });
            }
            catch (DivideByZeroException ex)
            {
                return new ApiResponse(400, "Division by zero error", new
                {
                    Expression = expressionText,
                    Error = ex.Message,
                    IsDivisionByZero = true
                });
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error calculating expression: {ex.Message}");
            }
        }
        #endregion

        #region Helper Methods for Calculation
        private string PreprocessExpression(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
                return expression;

            expression = expression.Trim();

            // Handle percentage: x% becomes x/100
            var percentagePattern = @"(\d+(\.\d+)?)\s*%";
            expression = Regex.Replace(expression, percentagePattern,
                match => $"({match.Groups[1].Value}/100)");

            // Handle percentage of: x% of y becomes (x/100)*y
            var percentageOfPattern = @"(\d+(\.\d+)?)%\s+of\s+([a-zA-Z0-9_]+|\([^)]+\))";
            expression = Regex.Replace(expression, percentageOfPattern,
                match => $"({match.Groups[1].Value}/100)*{match.Groups[3].Value}");

            // Handle power: ^ to Math.Pow
            expression = ConvertPowerToMathPow(expression);

            // Handle modulus: % to MOD
            expression = expression.Replace(" % ", " MOD ");

            // Handle sqrt
            expression = Regex.Replace(expression, @"sqrt\(([^)]+)\)",
                match => $"Math.Sqrt({match.Groups[1].Value})", RegexOptions.IgnoreCase);

            // Handle abs
            expression = Regex.Replace(expression, @"abs\(([^)]+)\)",
                match => $"Math.Abs({match.Groups[1].Value})", RegexOptions.IgnoreCase);

            // Handle log and ln
            expression = Regex.Replace(expression, @"log\(([^)]+)\)",
                match => $"Math.Log10({match.Groups[1].Value})", RegexOptions.IgnoreCase);
            expression = Regex.Replace(expression, @"ln\(([^)]+)\)",
                match => $"Math.Log({match.Groups[1].Value})", RegexOptions.IgnoreCase);

            return expression;
        }

        private string ConvertPowerToMathPow(string expression)
        {
            var powerPattern = @"([a-zA-Z0-9_\.]+|\d+(\.\d+)?|\([^)]+\))\s*\^\s*([a-zA-Z0-9_\.]+|\d+(\.\d+)?|\([^)]+\))";
            var regex = new Regex(powerPattern);
            var matches = regex.Matches(expression);

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    var baseValue = match.Groups[1].Value;
                    var exponent = match.Groups[3].Value;
                    var replacement = $"Math.Pow({baseValue}, {exponent})";
                    expression = expression.Replace(match.Value, replacement);
                }
            }

            return expression;
        }

        private object AdvancedEvaluate(string expression)
        {
            try
            {
                if (expression.Contains(" MOD "))
                {
                    var parts = expression.Split(new[] { " MOD " }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 2)
                    {
                        var left = EvaluateSimpleExpression(parts[0]);
                        var right = EvaluateSimpleExpression(parts[1]);

                        if (IsNumeric(left) && IsNumeric(right))
                        {
                            var leftNum = Convert.ToDecimal(left);
                            var rightNum = Convert.ToDecimal(right);

                            if (rightNum == 0)
                                throw new DivideByZeroException("Modulus by zero");

                            return leftNum % rightNum;
                        }
                    }
                }

                if (expression.Contains("Math."))
                {
                    return EvaluateMathFunctions(expression);
                }

                return EvaluateSimpleExpression(expression);
            }
            catch (Exception ex)
            {
                throw new Exception($"Advanced evaluation error: {ex.Message}");
            }
        }

        private object EvaluateSimpleExpression(string expression)
        {
            try
            {
                var dataTable = new DataTable();
                var result = dataTable.Compute(expression, "");
                return ConvertResultToAppropriateType(result);
            }
            catch
            {
                throw new Exception($"Cannot evaluate expression: {expression}");
            }
        }

        private object EvaluateMathFunctions(string expression)
        {
            try
            {
                if (expression.Contains("Math.Pow"))
                {
                    var pattern = @"Math\.Pow\(([^,]+),\s*([^)]+)\)";
                    var match = Regex.Match(expression, pattern);
                    if (match.Success)
                    {
                        var baseVal = Convert.ToDouble(EvaluateSimpleExpression(match.Groups[1].Value));
                        var expVal = Convert.ToDouble(EvaluateSimpleExpression(match.Groups[2].Value));
                        return Math.Pow(baseVal, expVal);
                    }
                }
                else if (expression.Contains("Math.Sqrt"))
                {
                    var pattern = @"Math\.Sqrt\(([^)]+)\)";
                    var match = Regex.Match(expression, pattern);
                    if (match.Success)
                    {
                        var val = Convert.ToDouble(EvaluateSimpleExpression(match.Groups[1].Value));
                        return Math.Sqrt(val);
                    }
                }
                else if (expression.Contains("Math.Abs"))
                {
                    var pattern = @"Math\.Abs\(([^)]+)\)";
                    var match = Regex.Match(expression, pattern);
                    if (match.Success)
                    {
                        var val = Convert.ToDouble(EvaluateSimpleExpression(match.Groups[1].Value));
                        return Math.Abs(val);
                    }
                }
                else if (expression.Contains("Math.Log10"))
                {
                    var pattern = @"Math\.Log10\(([^)]+)\)";
                    var match = Regex.Match(expression, pattern);
                    if (match.Success)
                    {
                        var val = Convert.ToDouble(EvaluateSimpleExpression(match.Groups[1].Value));
                        return Math.Log10(val);
                    }
                }
                else if (expression.Contains("Math.Log"))
                {
                    var pattern = @"Math\.Log\(([^)]+)\)";
                    var match = Regex.Match(expression, pattern);
                    if (match.Success)
                    {
                        var val = Convert.ToDouble(EvaluateSimpleExpression(match.Groups[1].Value));
                        return Math.Log(val);
                    }
                }

                return EvaluateSimpleExpression(expression);
            }
            catch (Exception ex)
            {
                throw new Exception($"Math function evaluation error: {ex.Message}");
            }
        }

        private object ConvertResultToAppropriateType(object result)
        {
            if (result == null)
                return null;

            try
            {
                if (result is decimal || result is double || result is float)
                {
                    var decimalResult = Convert.ToDecimal(result);
                    return Math.Round(decimalResult, 6);
                }
                else if (result is int || result is long || result is short)
                {
                    return Convert.ToInt32(result);
                }
                else if (result is bool)
                {
                    return result;
                }
                else
                {
                    return Convert.ToDecimal(result);
                }
            }
            catch
            {
                return result;
            }
        }

        private bool IsNumeric(object value)
        {
            if (value == null)
                return false;

            return value is sbyte || value is byte || value is short || value is ushort ||
                   value is int || value is uint || value is long || value is ulong ||
                   value is float || value is double || value is decimal;
        }

        private bool ContainsDivisionByZero(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return false;

            var patterns = new string[]
            {
                @"/\s*0\b",
                @"/\s*\(0\)",
                @"/\s*0\.0",
                @"/\s*0\.00",
                @"÷\s*0",
            };

            foreach (var pattern in patterns)
            {
                if (Regex.IsMatch(expression, pattern))
                    return true;
            }

            return CheckComplexDivisionByZero(expression);
        }

        private bool CheckComplexDivisionByZero(string expression)
        {
            try
            {
                var divisionPattern = @"/([^+\-*/()]+|\([^)]+\))";
                var matches = Regex.Matches(expression, divisionPattern);

                foreach (Match match in matches)
                {
                    if (match.Groups.Count > 1)
                    {
                        var divisor = match.Groups[1].Value.Trim();

                        try
                        {
                            var dataTable = new DataTable();
                            var divisorValue = dataTable.Compute(divisor, "");

                            if (IsZero(divisorValue))
                                return true;
                        }
                        catch
                        {
                            continue;
                        }
                    }
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool IsZero(object value)
        {
            if (value == null)
                return false;

            try
            {
                if (value is decimal decimalValue)
                    return decimalValue == 0;
                if (value is double doubleValue)
                    return doubleValue == 0;
                if (value is float floatValue)
                    return floatValue == 0;
                if (value is int intValue)
                    return intValue == 0;
                if (value is long longValue)
                    return longValue == 0;

                return Convert.ToDecimal(value) == 0;
            }
            catch
            {
                return false;
            }
        }

        private bool ContainsModulusByZero(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return false;

            var patterns = new string[]
            {
                @"MOD\s+0\b",
                @"%\s*0\b",
                @"MOD\s+\(0\)",
                @"%\s*\(0\)",
                @"MOD\s+0\.0",
                @"%\s*0\.0"
            };

            foreach (var pattern in patterns)
            {
                if (Regex.IsMatch(expression, pattern, RegexOptions.IgnoreCase))
                    return true;
            }

            return false;
        }

        private bool ContainsInvalidOperations(string expression)
        {
            if (string.IsNullOrEmpty(expression))
                return false;

            var invalidPatterns = new string[]
            {
                @"Math\.Sqrt\(-?\d+\)",
                @"Math\.Log10\(-?\d+\)",
                @"Math\.Log\(-?\d+\)",
                @"Math\.Pow\(-?\d+,\s*-?\d+\)"
            };

            foreach (var pattern in invalidPatterns)
            {
                if (Regex.IsMatch(expression, pattern))
                    return true;
            }

            return false;
        }

        private object SafeEvaluateExpression(string expression)
        {
            try
            {
                expression = expression?.Replace(" ", "");

                if (string.IsNullOrEmpty(expression))
                    return null;

                if (ContainsDivisionByZero(expression))
                {
                    throw new DivideByZeroException($"Division by zero in expression: {expression}");
                }

                var dataTable = new DataTable();
                var result = dataTable.Compute(expression, "");

                if (result is double doubleResult)
                {
                    if (double.IsInfinity(doubleResult))
                        throw new DivideByZeroException("Result is infinity");
                    if (double.IsNaN(doubleResult))
                        throw new ArithmeticException("Result is NaN");

                    return Math.Round(doubleResult, 6);
                }

                try
                {
                    return Convert.ToDecimal(result);
                }
                catch
                {
                    return result;
                }
            }
            catch (DivideByZeroException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error evaluating expression: {ex.Message}");
            }
        }

        private async Task<object> GetFieldValueAsync(int fieldId)
        {
            // Implement based on your data structure
            return null;
        }

        private object GenerateSampleValue(string fieldType)
        {
            if (string.IsNullOrEmpty(fieldType))
                return 0;

            var random = new Random();

            switch (fieldType.ToLower())
            {
                case "number":
                case "integer":
                case "decimal":
                case "currency":
                    return random.Next(1, 100);

                case "percentage":
                    return random.Next(1, 100);

                case "boolean":
                case "checkbox":
                    return random.Next(0, 2) == 1;

                case "date":
                    return DateTime.Now.AddDays(random.Next(-30, 30)).ToString("yyyy-MM-dd");

                case "text":
                case "string":
                case "varchar":
                    return $"Sample_{random.Next(1, 100)}";

                default:
                    return random.Next(1, 100);
            }
        }

        private string GenerateCalculationSteps(string expressionText, string processedExpression, Dictionary<string, object> fieldValues)
        {
            var steps = new List<string>();

            if (fieldValues.Any())
            {
                steps.Add("Step 1: Replace field codes with values:");
                foreach (var kvp in fieldValues)
                {
                    steps.Add($"  {kvp.Key} = {kvp.Value}");
                }
            }

            steps.Add($"Step 2: Preprocessed expression: {processedExpression}");
            steps.Add($"Step 3: Calculate: {processedExpression}");

            return string.Join("\n", steps);
        }

        private string GenerateCalculationSteps(string expression, Dictionary<string, object> values)
        {
            var steps = new List<string>();
            var currentExpression = expression;

            foreach (var kvp in values)
            {
                var step = $"Replace '{kvp.Key}' with value '{kvp.Value}'";
                steps.Add(step);

                currentExpression = currentExpression
                    .Replace($"[{kvp.Key}]", kvp.Value?.ToString())
                    .Replace($"{{{kvp.Key}}}", kvp.Value?.ToString())
                    .Replace(kvp.Key, kvp.Value?.ToString());
            }

            steps.Add($"Final expression: {currentExpression}");
            steps.Add($"Result: {EvaluateSimpleExpression(currentExpression)}");

            return string.Join("\n", steps);
        }

        private List<string> ExtractOperationsUsed(string expression)
        {
            var operations = new List<string>();

            if (expression.Contains("+"))
                operations.Add("Addition (+)");
            if (expression.Contains("-"))
                operations.Add("Subtraction (-)");
            if (expression.Contains("*"))
                operations.Add("Multiplication (*)");
            if (expression.Contains("/"))
                operations.Add("Division (/)");
            if (expression.Contains("%") && !expression.Contains("% of"))
                operations.Add("Modulus (%)");
            if (expression.Contains("% of"))
                operations.Add("Percentage of (%)");
            if (expression.Contains("^"))
                operations.Add("Power (^)");
            if (expression.Contains("sqrt", StringComparison.OrdinalIgnoreCase))
                operations.Add("Square Root (sqrt)");
            if (expression.Contains("abs", StringComparison.OrdinalIgnoreCase))
                operations.Add("Absolute Value (abs)");
            if (expression.Contains("log", StringComparison.OrdinalIgnoreCase))
                operations.Add("Logarithm (log)");
            if (expression.Contains("ln", StringComparison.OrdinalIgnoreCase))
                operations.Add("Natural Logarithm (ln)");

            return operations;
        }
        #endregion

        #region Private Helper Methods
        private FormulaDto ToDto(FORMULAS entity)
        {
            if (entity == null) return null;

            return new FormulaDto
            {
                Id = entity.Id,
                FormBuilderId = entity.FormBuilderId,
                FormBuilderName = entity.FORM_BUILDER?.FormName,
                Name = entity.Name,
                Code = entity.Code,
                ExpressionText = entity.ExpressionText,
                ResultFieldId = entity.ResultFieldId,
                ResultFieldName = entity.RESULT_FIELD?.FieldName,
                ResultFieldCode = entity.RESULT_FIELD?.FieldCode,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate,
                VariableCount = entity.FORMULA_VARIABLES?.Count ?? 0
            };
        }

        private void MapUpdate(UpdateFormulaDto dto, FORMULAS entity)
        {
            if (!string.IsNullOrEmpty(dto.Name))
                entity.Name = dto.Name;

            if (!string.IsNullOrEmpty(dto.Code))
                entity.Code = dto.Code;

            if (!string.IsNullOrEmpty(dto.ExpressionText))
                entity.ExpressionText = dto.ExpressionText;

            if (dto.ResultFieldId.HasValue)
                entity.ResultFieldId = dto.ResultFieldId.Value;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;
        }
        #endregion
    }
}