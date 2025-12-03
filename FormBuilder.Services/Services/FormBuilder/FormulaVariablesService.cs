//using formBuilder.Domian.Interfaces;
//using FormBuilder.API.Models;
//using FormBuilder.Core.DTOS.FormBuilder;
//using FormBuilder.Domain.Interfaces.Repositories;
//using FormBuilder.Domain.Interfaces.Services;
//using FormBuilder.Domian.Entitys.froms;
//using FormBuilder.Domian.Interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace FormBuilder.Services
//{
//    public class FormulaVariablesService : IFormulaVariablesService
//    {
//        private readonly IunitOfwork _unitOfWork;
//        private readonly IFormulaVariablesRepository _formulaVariablesRepository;
//        private readonly IFormulasRepository _formulasRepository;

//        public FormulaVariablesService(
//            IunitOfwork unitOfWork,
//            IFormulaVariablesRepository formulaVariablesRepository,
//            IFormulasRepository formulasRepository)
//        {
//            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
//            _formulaVariablesRepository = formulaVariablesRepository ?? throw new ArgumentNullException(nameof(formulaVariablesRepository));
//            _formulasRepository = formulasRepository ?? throw new ArgumentNullException(nameof(formulasRepository));
//        }

//        #region Basic CRUD Operations
//        public async Task<ApiResponse> GetAllByFormulaIdAsync(int formulaId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetByFormulaIdAsync(formulaId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Formula variables retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variables: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetByIdAsync(int id)
//        {
//            try
//            {
//                var variable = await _formulaVariablesRepository.GetByIdAsync(id);
//                if (variable == null)
//                    return new ApiResponse(404, "Formula variable not found");

//                var variableDto = ToDto(variable);
//                return new ApiResponse(200, "Formula variable retrieved successfully", variableDto);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variable: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetByIdWithDetailsAsync(int id)
//        {
//            try
//            {
//                var variable = await _formulaVariablesRepository.GetByIdWithDetailsAsync(id);
//                if (variable == null)
//                    return new ApiResponse(404, "Formula variable not found");

//                var variableDto = ToDto(variable);
//                return new ApiResponse(200, "Formula variable with details retrieved successfully", variableDto);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variable with details: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> CreateAsync(CreateFormulaVariableDto createDto)
//        {
//            try
//            {
//                if (createDto == null)
//                    return new ApiResponse(400, "DTO is required");

//                // Validate formula exists
//                var formula = await _formulasRepository.GetByIdAsync(createDto.FormulaId);
//                if (formula == null)
//                    return new ApiResponse(404, "Formula not found");

//                // Validate source field exists and belongs to the same form builder
//                var fieldBelongsToForm = await _formulasRepository.FieldBelongsToFormBuilderAsync(
//                    createDto.SourceFieldId, formula.FormBuilderId);

//                if (!fieldBelongsToForm)
//                    return new ApiResponse(400, "Source field not found or doesn't belong to the same form builder");

//                // Check if variable name already exists in this formula
//                var nameExists = await _formulaVariablesRepository.VariableNameExistsAsync(
//                    createDto.VariableName, createDto.FormulaId);

//                if (nameExists)
//                    return new ApiResponse(400, "Variable name already exists in this formula");

//                // Create formula variable entity
//                var entity = new FORMULA_VARIABLES
//                {
//                    FormulaId = createDto.FormulaId,
//                    VariableName = createDto.VariableName,
//                    SourceFieldId = createDto.SourceFieldId,
//                    IsActive = createDto.IsActive,
//                    CreatedDate = DateTime.UtcNow,
//                    UpdatedDate = DateTime.UtcNow
//                };

//                _unitOfWork.FormulaVariablesRepository.Add(entity);
//                await _unitOfWork.CompleteAsyn();

//                var createdVariable = await _formulaVariablesRepository.GetByIdWithDetailsAsync(entity.Id);
//                return new ApiResponse(200, "Formula variable created successfully", ToDto(createdVariable));
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error creating formula variable: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> CreateBulkAsync(BulkCreateFormulaVariablesDto bulkDto)
//        {
//            try
//            {
//                if (bulkDto == null || !bulkDto.Variables.Any())
//                    return new ApiResponse(400, "DTO is required or variables list is empty");

//                // Validate formula exists
//                var formula = await _formulasRepository.GetByIdAsync(bulkDto.FormulaId);
//                if (formula == null)
//                    return new ApiResponse(404, "Formula not found");

//                var createdVariables = new List<FormulaVariableDto>();
//                var errors = new List<string>();
//                var successCount = 0;

//                foreach (var variableDto in bulkDto.Variables)
//                {
//                    try
//                    {
//                        // Validate source field exists and belongs to the same form builder
//                        var fieldBelongsToForm = await _formulasRepository.FieldBelongsToFormBuilderAsync(
//                            variableDto.SourceFieldId, formula.FormBuilderId);

//                        if (!fieldBelongsToForm)
//                        {
//                            errors.Add($"Variable '{variableDto.VariableName}': Source field not found or doesn't belong to the form builder");
//                            continue;
//                        }

//                        // Check if variable name already exists in this formula
//                        var nameExists = await _formulaVariablesRepository.VariableNameExistsAsync(
//                            variableDto.VariableName, bulkDto.FormulaId);

//                        if (nameExists)
//                        {
//                            errors.Add($"Variable '{variableDto.VariableName}': Variable name already exists in this formula");
//                            continue;
//                        }

//                        // Create formula variable entity
//                        var entity = new FORMULA_VARIABLES
//                        {
//                            FormulaId = bulkDto.FormulaId,
//                            VariableName = variableDto.VariableName,
//                            SourceFieldId = variableDto.SourceFieldId,
//                            IsActive = variableDto.IsActive,
//                            CreatedDate = DateTime.UtcNow,
//                            UpdatedDate = DateTime.UtcNow
//                        };

//                        _unitOfWork.FormulaVariablesRepository.Add(entity);
//                        successCount++;
//                    }
//                    catch (Exception ex)
//                    {
//                        errors.Add($"Variable '{variableDto.VariableName}': {ex.Message}");
//                    }
//                }

//                await _unitOfWork.CompleteAsyn();

//                // Get created variables
//                var variables = await _formulaVariablesRepository.GetByFormulaIdAsync(bulkDto.FormulaId);
//                createdVariables = variables.Select(ToDto).ToList();

//                return new ApiResponse(200, "Bulk creation completed", new
//                {
//                    TotalRequested = bulkDto.Variables.Count,
//                    SuccessCount = successCount,
//                    ErrorCount = errors.Count,
//                    Errors = errors,
//                    CreatedVariables = createdVariables
//                });
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error in bulk creation: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormulaVariableDto updateDto)
//        {
//            try
//            {
//                if (updateDto == null)
//                    return new ApiResponse(400, "DTO is required");

//                var entity = await _formulaVariablesRepository.GetByIdAsync(id);
//                if (entity == null)
//                    return new ApiResponse(404, "Formula variable not found");

//                var formula = await _formulasRepository.GetByIdAsync(entity.FormulaId);

//                // Update variable name if provided
//                if (!string.IsNullOrEmpty(updateDto.VariableName) && updateDto.VariableName != entity.VariableName)
//                {
//                    var nameExists = await _formulaVariablesRepository.VariableNameExistsAsync(
//                        updateDto.VariableName, entity.FormulaId, id);

//                    if (nameExists)
//                        return new ApiResponse(400, "Variable name already exists in this formula");

//                    entity.VariableName = updateDto.VariableName;
//                }

//                // Update source field if provided
//                if (updateDto.SourceFieldId.HasValue)
//                {
//                    var fieldBelongsToForm = await _formulasRepository.FieldBelongsToFormBuilderAsync(
//                        updateDto.SourceFieldId.Value, formula.FormBuilderId);

//                    if (!fieldBelongsToForm)
//                        return new ApiResponse(400, "Source field not found or doesn't belong to the form builder");

//                    entity.SourceFieldId = updateDto.SourceFieldId.Value;
//                }

//                // Update active status if provided
//                if (updateDto.IsActive.HasValue)
//                    entity.IsActive = updateDto.IsActive.Value;

//                entity.UpdatedDate = DateTime.UtcNow;

//                _unitOfWork.FormulaVariablesRepository.Update(entity);
//                await _unitOfWork.CompleteAsyn();

//                var updatedVariable = await _formulaVariablesRepository.GetByIdWithDetailsAsync(id);
//                return new ApiResponse(200, "Formula variable updated successfully", ToDto(updatedVariable));
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error updating formula variable: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> DeleteAsync(int id)
//        {
//            try
//            {
//                var entity = await _formulaVariablesRepository.GetByIdAsync(id);
//                if (entity == null)
//                    return new ApiResponse(404, "Formula variable not found");

//                _unitOfWork.FormulaVariablesRepository.Delete(entity);
//                await _unitOfWork.CompleteAsyn();

//                return new ApiResponse(200, "Formula variable deleted successfully");
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error deleting formula variable: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> DeleteByFormulaIdAsync(int formulaId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetByFormulaIdAsync(formulaId);
//                if (!variables.Any())
//                    return new ApiResponse(404, "No formula variables found for this formula");

//                var variablesList = variables.ToList();
//                _unitOfWork.FormulaVariablesRepository.DeleteRange(variablesList);
//                await _unitOfWork.CompleteAsyn();

//                return new ApiResponse(200, $"Deleted {variablesList.Count} formula variables successfully");
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error deleting formula variables: {ex.Message}");
//            }
//        }
//        #endregion

//        #region Query Operations
//        public async Task<ApiResponse> GetBySourceFieldIdAsync(int sourceFieldId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetBySourceFieldIdAsync(sourceFieldId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Formula variables by source field retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variables by source field: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetActiveByFormulaIdAsync(int formulaId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetActiveByFormulaIdAsync(formulaId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Active formula variables retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving active formula variables: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetInactiveByFormulaIdAsync(int formulaId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetInactiveByFormulaIdAsync(formulaId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Inactive formula variables retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving inactive formula variables: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> SearchVariablesAsync(int formulaId, string searchTerm)
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(searchTerm))
//                    return new ApiResponse(400, "Search term is required");

//                var variables = await _formulaVariablesRepository.SearchVariablesAsync(formulaId, searchTerm);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Formula variables search completed successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error searching formula variables: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetByVariableNameAsync(string variableName)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetAllAsync(
//                    fv => fv.VariableName.ToLower() == variableName.ToLower());

//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Formula variables by name retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variables by name: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetByFieldTypeAsync(string fieldType, int formulaId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetByFieldTypeAsync(fieldType, formulaId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Formula variables by field type retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variables by field type: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetVariablesWithoutSourceFieldAsync(int formulaId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetVariablesWithoutSourceFieldAsync(formulaId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Formula variables without source field retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variables without source field: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetByFormBuilderIdAsync(formBuilderId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Formula variables by form builder retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variables by form builder: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> CountByFormulaIdAsync(int formulaId)
//        {
//            try
//            {
//                var count = await _formulaVariablesRepository.CountAsync(fv => fv.FormulaId == formulaId);
//                return new ApiResponse(200, "Formula variables count retrieved successfully", count);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error counting formula variables: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> CountActiveByFormulaIdAsync(int formulaId)
//        {
//            try
//            {
//                var count = await _formulaVariablesRepository.CountActiveByFormulaIdAsync(formulaId);
//                return new ApiResponse(200, "Active formula variables count retrieved successfully", count);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error counting active formula variables: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> CountInactiveByFormulaIdAsync(int formulaId)
//        {
//            try
//            {
//                var count = await _formulaVariablesRepository.CountInactiveByFormulaIdAsync(formulaId);
//                return new ApiResponse(200, "Inactive formula variables count retrieved successfully", count);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error counting inactive formula variables: {ex.Message}");
//            }
//        }
//        #endregion

//        #region Validation Operations
//        public async Task<ApiResponse> VariableNameExistsAsync(string variableName, int formulaId, int? excludeId = null)
//        {
//            try
//            {
//                var exists = await _formulaVariablesRepository.VariableNameExistsAsync(variableName, formulaId, excludeId);
//                return new ApiResponse(200, "Variable name existence checked successfully", exists);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error checking variable name existence: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> IsSourceFieldUsedAsync(int fieldId)
//        {
//            try
//            {
//                var isUsed = await _formulaVariablesRepository.IsSourceFieldUsedAsync(fieldId);
//                return new ApiResponse(200, "Source field usage checked successfully", isUsed);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error checking source field usage: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> IsSourceFieldUsedInOtherFormulasAsync(int fieldId, int excludeFormulaId)
//        {
//            try
//            {
//                var isUsed = await _formulaVariablesRepository.IsSourceFieldUsedInOtherFormulasAsync(fieldId, excludeFormulaId);
//                return new ApiResponse(200, "Source field usage in other formulas checked successfully", isUsed);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error checking source field usage in other formulas: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> ValidateVariableAsync(CreateFormulaVariableDto createDto)
//        {
//            try
//            {
//                if (createDto == null)
//                    return new ApiResponse(400, "DTO is required");

//                // Validate formula exists
//                var formula = await _formulasRepository.GetByIdAsync(createDto.FormulaId);
//                if (formula == null)
//                    return new ApiResponse(404, "Formula not found");

//                // Validate source field exists and belongs to the same form builder
//                var fieldBelongsToForm = await _formulasRepository.FieldBelongsToFormBuilderAsync(
//                    createDto.SourceFieldId, formula.FormBuilderId);

//                if (!fieldBelongsToForm)
//                    return new ApiResponse(400, "Source field not found or doesn't belong to the form builder");

//                // Check if variable name already exists
//                var nameExists = await _formulaVariablesRepository.VariableNameExistsAsync(
//                    createDto.VariableName, createDto.FormulaId);

//                if (nameExists)
//                    return new ApiResponse(400, "Variable name already exists in this formula");

//                return new ApiResponse(200, "Formula variable is valid", true);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error validating formula variable: {ex.Message}", false);
//            }
//        }

//        public async Task<ApiResponse> IsVariableValidForFormulaAsync(int formulaId, string variableName, int sourceFieldId)
//        {
//            try
//            {
//                var isValid = await _formulaVariablesRepository.IsVariableValidForFormulaAsync(formulaId, variableName, sourceFieldId);
//                return new ApiResponse(200, "Variable validation completed", isValid);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error validating variable: {ex.Message}", false);
//            }
//        }
//        #endregion

//        #region Utility Operations
//        public async Task<ApiResponse> GetFormulaVariablesWithDetailsAsync(int formulaId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetFormulaVariablesWithDetailsAsync(formulaId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Formula variables with details retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variables with details: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> HasFormulaVariablesAsync(int formulaId)
//        {
//            try
//            {
//                var hasVariables = await _formulaVariablesRepository.HasFormulaVariablesAsync(formulaId);
//                return new ApiResponse(200, "Formula variables existence checked successfully", hasVariables);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error checking formula variables existence: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> HasActiveVariablesAsync(int formulaId)
//        {
//            try
//            {
//                var hasActive = await _formulaVariablesRepository.HasActiveVariablesAsync(formulaId);
//                return new ApiResponse(200, "Active variables existence checked successfully", hasActive);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error checking active variables existence: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> UpdateVariableNameAsync(int id, string variableName)
//        {
//            try
//            {
//                if (string.IsNullOrWhiteSpace(variableName))
//                    return new ApiResponse(400, "Variable name is required");

//                var entity = await _formulaVariablesRepository.GetByIdAsync(id);
//                if (entity == null)
//                    return new ApiResponse(404, "Formula variable not found");

//                if (variableName == entity.VariableName)
//                    return new ApiResponse(200, "Variable name is already the same", ToDto(entity));

//                var nameExists = await _formulaVariablesRepository.VariableNameExistsAsync(
//                    variableName, entity.FormulaId, id);

//                if (nameExists)
//                    return new ApiResponse(400, "Variable name already exists in this formula");

//                entity.VariableName = variableName;
//                entity.UpdatedDate = DateTime.UtcNow;

//                _unitOfWork.FormulaVariablesRepository.Update(entity);
//                await _unitOfWork.CompleteAsyn();

//                var updatedVariable = await _formulaVariablesRepository.GetByIdWithDetailsAsync(id);
//                return new ApiResponse(200, "Variable name updated successfully", ToDto(updatedVariable));
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error updating variable name: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> UpdateSourceFieldAsync(int id, int sourceFieldId)
//        {
//            try
//            {
//                var entity = await _formulaVariablesRepository.GetByIdAsync(id);
//                if (entity == null)
//                    return new ApiResponse(404, "Formula variable not found");

//                var formula = await _formulasRepository.GetByIdAsync(entity.FormulaId);

//                var fieldBelongsToForm = await _formulasRepository.FieldBelongsToFormBuilderAsync(
//                    sourceFieldId, formula.FormBuilderId);

//                if (!fieldBelongsToForm)
//                    return new ApiResponse(400, "Source field not found or doesn't belong to the form builder");

//                entity.SourceFieldId = sourceFieldId;
//                entity.UpdatedDate = DateTime.UtcNow;

//                _unitOfWork.FormulaVariablesRepository.Update(entity);
//                await _unitOfWork.CompleteAsyn();

//                var updatedVariable = await _formulaVariablesRepository.GetByIdWithDetailsAsync(id);
//                return new ApiResponse(200, "Source field updated successfully", ToDto(updatedVariable));
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error updating source field: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetVariableNamesInFormulaAsync(int formulaId)
//        {
//            try
//            {
//                var variableNames = await _formulaVariablesRepository.GetVariableNamesInFormulaAsync(formulaId);
//                return new ApiResponse(200, "Variable names retrieved successfully", variableNames);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving variable names: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetVariableMappingsForFormulaAsync(int formulaId)
//        {
//            try
//            {
//                var mappings = await _formulaVariablesRepository.GetVariableMappingsForFormulaAsync(formulaId);
//                return new ApiResponse(200, "Variable mappings retrieved successfully", mappings);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving variable mappings: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetVariableCountByFormulasAsync(List<int> formulaIds)
//        {
//            try
//            {
//                if (formulaIds == null || !formulaIds.Any())
//                    return new ApiResponse(400, "Formula IDs are required");

//                var counts = await _formulaVariablesRepository.GetVariableCountByFormulaAsync(formulaIds);
//                return new ApiResponse(200, "Variable counts retrieved successfully", counts);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving variable counts: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetVariablesWithFormulaDetailsAsync(int formulaId)
//        {
//            try
//            {
//                var variables = await _formulaVariablesRepository.GetVariablesWithFormulaDetailsAsync(formulaId);
//                var variableDtos = variables.Select(ToDto).ToList();
//                return new ApiResponse(200, "Variables with formula details retrieved successfully", variableDtos);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving variables with formula details: {ex.Message}");
//            }
//        }
//        #endregion

//        #region Status Management
//        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
//        {
//            try
//            {
//                var entity = await _formulaVariablesRepository.GetByIdAsync(id);
//                if (entity == null)
//                    return new ApiResponse(404, "Formula variable not found");

//                entity.IsActive = isActive;
//                entity.UpdatedDate = DateTime.UtcNow;

//                _unitOfWork.FormulaVariablesRepository.Update(entity);
//                await _unitOfWork.CompleteAsyn();

//                var message = isActive ? "activated" : "deactivated";
//                return new ApiResponse(200, $"Formula variable {message} successfully", ToDto(entity));
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error toggling formula variable active status: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> BatchToggleActiveAsync(List<int> variableIds, bool isActive)
//        {
//            try
//            {
//                if (variableIds == null || !variableIds.Any())
//                    return new ApiResponse(400, "Variable IDs are required");

//                var updatedCount = 0;
//                var failedIds = new List<int>();

//                foreach (var variableId in variableIds)
//                {
//                    try
//                    {
//                        var variable = await _formulaVariablesRepository.GetByIdAsync(variableId);
//                        if (variable != null)
//                        {
//                            variable.IsActive = isActive;
//                            variable.UpdatedDate = DateTime.UtcNow;
//                            _unitOfWork.FormulaVariablesRepository.Update(variable);
//                            updatedCount++;
//                        }
//                        else
//                        {
//                            failedIds.Add(variableId);
//                        }
//                    }
//                    catch
//                    {
//                        failedIds.Add(variableId);
//                    }
//                }

//                await _unitOfWork.CompleteAsyn();

//                var result = new
//                {
//                    UpdatedCount = updatedCount,
//                    FailedCount = failedIds.Count,
//                    FailedIds = failedIds,
//                    Message = $"Successfully updated {updatedCount} variables. Failed to update {failedIds.Count} variables."
//                };

//                return new ApiResponse(200, "Batch update completed", result);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error in batch update: {ex.Message}");
//            }
//        }

//        public async Task<ApiResponse> GetStatisticsByFormulaIdAsync(int formulaId)
//        {
//            try
//            {
//                var activeCount = await _formulaVariablesRepository.CountActiveByFormulaIdAsync(formulaId);
//                var inactiveCount = await _formulaVariablesRepository.CountInactiveByFormulaIdAsync(formulaId);
//                var totalCount = activeCount + inactiveCount;

//                var statistics = new
//                {
//                    FormulaId = formulaId,
//                    TotalVariables = totalCount,
//                    ActiveVariables = activeCount,
//                    InactiveVariables = inactiveCount,
//                    HasVariables = totalCount > 0,
//                    HasActiveVariables = activeCount > 0
//                };

//                return new ApiResponse(200, "Formula variables statistics retrieved successfully", statistics);
//            }
//            catch (Exception ex)
//            {
//                return new ApiResponse(500, $"Error retrieving formula variables statistics: {ex.Message}");
//            }
//        }
//        #endregion

//        #region Private Helper Methods
//        private FormulaVariableDto ToDto(FORMULA_VARIABLES entity)
//        {
//            if (entity == null) return null;

//            var dto = new FormulaVariableDto
//            {
//                Id = entity.Id,
//                FormulaId = entity.FormulaId,
//                FormulaName = entity.FORMULAS?.Name,
//                FormulaCode = entity.FORMULAS?.Code,
//                VariableName = entity.VariableName,
//                SourceFieldId = entity.SourceFieldId,
//                SourceFieldCode = entity.FORM_FIELDS?.FieldCode,
//                SourceFieldName = entity.FORM_FIELDS?.FieldName,
//                SourceFieldType = entity.FORM_FIELDS?.FIELD_TYPES?.TypeName,
//                IsActive = entity.IsActive,
//                CreatedDate = entity.CreatedDate,
//                UpdatedDate = entity.UpdatedDate
//            };

//            // Add extended information if needed
//            if (entity.FORMULAS != null)
//            {
//                dto.FormulaInfo = new FormulaInfoDto
//                {
//                    Id = entity.FORMULAS.Id,
//                    Name = entity.FORMULAS.Name,
//                    Code = entity.FORMULAS.Code,
//                    Expression = entity.FORMULAS.Expression,
//                    FormBuilderId = entity.FORMULAS.FormBuilderId,
//                    IsActive = entity.FORMULAS.IsActive
//                };
//            }

//            if (entity.FORM_FIELDS != null)
//            {
//                dto.FieldInfo = new FieldInfoDto
//                {
//                    Id = entity.FORM_FIELDS.Id,
//                    FieldCode = entity.FORM_FIELDS.FieldCode,
//                    FieldName = entity.FORM_FIELDS.FieldName,
//                    FieldType = entity.FORM_FIELDS.FIELD_TYPES?.TypeName,
//                    FieldTypeId = entity.FORM_FIELDS.FieldTypeId,
//                    IsRequired = entity.FORM_FIELDS.IsRequired,
//                    IsActive = entity.FORM_FIELDS.IsActive
//                };
//            }

//            return dto;
//        }

//        #endregion
//    }
//}