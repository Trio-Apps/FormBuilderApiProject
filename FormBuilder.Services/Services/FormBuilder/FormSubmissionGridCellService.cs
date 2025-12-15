using formBuilder.Domian.Interfaces;
using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domian.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionGridCellService : IFormSubmissionGridCellService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionGridCellService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var cells = await _unitOfWork.FormSubmissionGridCellRepository.GetAllAsync();
                var cellDtos = cells.Select(MapToDto).ToList();
                return new ApiResponse(200, "Grid cells retrieved successfully", cellDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid cells: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var cell = await _unitOfWork.FormSubmissionGridCellRepository.GetByIdAsync(id);
                if (cell == null)
                    return new ApiResponse(404, "Grid cell not found");

                var cellDto = MapToDto(cell);
                return new ApiResponse(200, "Grid cell retrieved successfully", cellDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid cell: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByRowIdAsync(int rowId)
        {
            try
            {
                var cells = await _unitOfWork.FormSubmissionGridCellRepository.GetByRowIdAsync(rowId);
                var cellDtos = cells.Select(MapToDto).ToList();
                return new ApiResponse(200, "Grid cells retrieved successfully", cellDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid cells: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByRowAndColumnAsync(int rowId, int columnId)
        {
            try
            {
                var cell = await _unitOfWork.FormSubmissionGridCellRepository.GetByRowAndColumnAsync(rowId, columnId);
                if (cell == null)
                    return new ApiResponse(404, "Grid cell not found");

                var cellDto = MapToDto(cell);
                return new ApiResponse(200, "Grid cell retrieved successfully", cellDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid cell: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionGridCellDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if row exists
                var row = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(createDto.RowId);
                if (row == null)
                    return new ApiResponse(404, "Grid row not found");

                // Check if column exists
                var column = await _unitOfWork.FormGridColumnRepository.GetByIdAsync(createDto.ColumnId);
                if (column == null)
                    return new ApiResponse(404, "Grid column not found");

                // Check if cell already exists
                var existingCell = await _unitOfWork.FormSubmissionGridCellRepository
                    .GetByRowAndColumnAsync(createDto.RowId, createDto.ColumnId);
                if (existingCell != null)
                    return new ApiResponse(400, "Cell already exists for this row and column");

                var entity = new FORM_SUBMISSION_GRID_CELLS
                {
                    RowId = createDto.RowId,
                    ColumnId = createDto.ColumnId,
                    ValueString = createDto.ValueString,
                    ValueNumber = createDto.ValueNumber,
                    ValueDate = createDto.ValueDate,
                    ValueBool = createDto.ValueBool,
                    ValueJson = createDto.ValueJson,
                    CreatedDate = DateTime.UtcNow
                };

                _unitOfWork.FormSubmissionGridCellRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                var createdEntity = await _unitOfWork.FormSubmissionGridCellRepository.GetByIdAsync(entity.Id);
                var cellDto = MapToDto(createdEntity);

                return new ApiResponse(201, "Grid cell created successfully", cellDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating grid cell: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionGridCellDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormSubmissionGridCellRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Grid cell not found");

                // Update fields
                if (updateDto.ValueString != null)
                    entity.ValueString = updateDto.ValueString;

                if (updateDto.ValueNumber.HasValue)
                    entity.ValueNumber = updateDto.ValueNumber.Value;

                if (updateDto.ValueDate.HasValue)
                    entity.ValueDate = updateDto.ValueDate.Value;

                if (updateDto.ValueBool.HasValue)
                    entity.ValueBool = updateDto.ValueBool.Value;

                if (updateDto.ValueJson != null)
                    entity.ValueJson = updateDto.ValueJson;

                entity.UpdatedDate = DateTime.UtcNow;
                _unitOfWork.FormSubmissionGridCellRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.FormSubmissionGridCellRepository.GetByIdAsync(id);
                var cellDto = MapToDto(updatedEntity);

                return new ApiResponse(200, "Grid cell updated successfully", cellDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating grid cell: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionGridCellRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Grid cell not found");

                _unitOfWork.FormSubmissionGridCellRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Grid cell deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting grid cell: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteByRowIdAsync(int rowId)
        {
            try
            {
                var row = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(rowId);
                if (row == null)
                    return new ApiResponse(404, "Grid row not found");

                var deletedCount = await _unitOfWork.FormSubmissionGridCellRepository.DeleteByRowIdAsync(rowId);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, $"{deletedCount} grid cells deleted successfully", deletedCount);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting grid cells: {ex.Message}");
            }
        }

        private FormSubmissionGridCellDto MapToDto(FORM_SUBMISSION_GRID_CELLS entity)
        {
            if (entity == null) return null;

            // Determine display value based on field type
            string displayValue = GetDisplayValue(entity);

            return new FormSubmissionGridCellDto
            {
                Id = entity.Id,
                RowId = entity.RowId,
                ColumnId = entity.ColumnId,
                ColumnCode = entity.FORM_GRID_COLUMNS?.ColumnCode ?? string.Empty,
                ColumnName = entity.FORM_GRID_COLUMNS?.ColumnName ?? string.Empty,
                FieldTypeId = entity.FORM_GRID_COLUMNS?.FieldTypeId,
                FieldTypeName = entity.FORM_GRID_COLUMNS?.FIELD_TYPES?.TypeName ?? string.Empty,
                ValueString = entity.ValueString,
                ValueNumber = entity.ValueNumber,
                ValueDate = entity.ValueDate,
                ValueBool = entity.ValueBool,
                ValueJson = entity.ValueJson,
                DisplayValue = displayValue ?? string.Empty,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }

        private string GetDisplayValue(FORM_SUBMISSION_GRID_CELLS cell)
        {
            if (!string.IsNullOrEmpty(cell.ValueString))
                return cell.ValueString;

            if (cell.ValueNumber.HasValue)
                return cell.ValueNumber.Value.ToString();

            if (cell.ValueDate.HasValue)
                return cell.ValueDate.Value.ToString("yyyy-MM-dd HH:mm:ss");

            if (cell.ValueBool.HasValue)
                return cell.ValueBool.Value ? "Yes" : "No";

            if (!string.IsNullOrEmpty(cell.ValueJson))
                return "[JSON Data]";

            return string.Empty;
        }
    }
}