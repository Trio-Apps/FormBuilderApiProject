using formBuilder.Domian.Interfaces;
using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domian.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionGridRowService : IFormSubmissionGridRowService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionGridRowService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetAllAsync();
                var rowDtos = rows.Select(MapToDto).ToList();
                return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid rows: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var row = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(id);
                if (row == null)
                    return new ApiResponse(404, "Grid row not found");

                var rowDto = MapToDto(row);
                return new ApiResponse(200, "Grid row retrieved successfully", rowDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid row: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetBySubmissionIdAsync(int submissionId)
        {
            try
            {
                var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetBySubmissionIdAsync(submissionId);
                var rowDtos = rows.Select(MapToDto).ToList();
                return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid rows: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByGridIdAsync(int gridId)
        {
            try
            {
                var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetByGridIdAsync(gridId);
                var rowDtos = rows.Select(MapToDto).ToList();
                return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid rows: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetBySubmissionAndGridAsync(int submissionId, int gridId)
        {
            try
            {
                var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetBySubmissionAndGridAsync(submissionId, gridId);
                var rowDtos = rows.Select(MapToDto).ToList();
                return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid rows: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveRowsAsync(int submissionId, int gridId)
        {
            try
            {
                var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetActiveRowsAsync(submissionId, gridId);
                var rowDtos = rows.Select(MapToDto).ToList();
                return new ApiResponse(200, "Active grid rows retrieved successfully", rowDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active grid rows: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionGridRowDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if submission exists
                var submission = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(createDto.SubmissionId);
                if (submission == null)
                    return new ApiResponse(404, "Form submission not found");

                // Check if grid exists
                var grid = await _unitOfWork.FormGridRepository.GetByIdAsync(createDto.GridId);
                if (grid == null)
                    return new ApiResponse(404, "Form grid not found");

                // Check if submission belongs to same form builder as grid
                if (submission.FormBuilderId != grid.FormBuilderId)
                    return new ApiResponse(400, "Submission and grid must belong to the same form builder");

                // Get row index if not provided
                var rowIndex = createDto.RowIndex ??
                    await _unitOfWork.FormSubmissionGridRowRepository.GetNextRowIndexAsync(createDto.SubmissionId, createDto.GridId);

                // Check if row already exists at this index
                var rowExists = await _unitOfWork.FormSubmissionGridRowRepository
                    .RowExistsAsync(createDto.SubmissionId, createDto.GridId, rowIndex);
                if (rowExists)
                    return new ApiResponse(400, $"Row already exists at index {rowIndex}");

                var entity = new FORM_SUBMISSION_GRID_ROWS
                {
                    SubmissionId = createDto.SubmissionId,
                    GridId = createDto.GridId,
                    RowIndex = rowIndex,
                    IsActive = createDto.IsActive,
                    CreatedDate = DateTime.UtcNow
                };

                _unitOfWork.FormSubmissionGridRowRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                var createdEntity = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(entity.Id);
                var rowDto = MapToDto(createdEntity);

                return new ApiResponse(201, "Grid row created successfully", rowDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating grid row: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateMultipleAsync(List<CreateFormSubmissionGridRowDto> createDtos)
        {
            try
            {
                if (createDtos == null || !createDtos.Any())
                    return new ApiResponse(400, "DTOs are required");

                var createdRows = new List<FormSubmissionGridRowDto>();

                foreach (var createDto in createDtos)
                {
                    var result = await CreateAsync(createDto);
                    if (result.StatusCode == 201)
                    {
                        createdRows.Add(result.Data as FormSubmissionGridRowDto);
                    }
                }

                return new ApiResponse(201, "Multiple grid rows created successfully", createdRows);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating multiple grid rows: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionGridRowDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Grid row not found");

                // Update row index if provided and different
                if (updateDto.RowIndex.HasValue && updateDto.RowIndex.Value != entity.RowIndex)
                {
                    // Check if row already exists at new index
                    var rowExists = await _unitOfWork.FormSubmissionGridRowRepository
                        .RowExistsAsync(entity.SubmissionId, entity.GridId, updateDto.RowIndex.Value);
                    if (rowExists)
                        return new ApiResponse(400, $"Row already exists at index {updateDto.RowIndex.Value}");

                    entity.RowIndex = updateDto.RowIndex.Value;
                }

                if (updateDto.IsActive.HasValue)
                    entity.IsActive = updateDto.IsActive.Value;

                entity.UpdatedDate = DateTime.UtcNow;
                _unitOfWork.FormSubmissionGridRowRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var updatedEntity = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(id);
                var rowDto = MapToDto(updatedEntity);

                return new ApiResponse(200, "Grid row updated successfully", rowDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating grid row: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Grid row not found");

                _unitOfWork.FormSubmissionGridRowRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Grid row deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting grid row: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteBySubmissionAndGridAsync(int submissionId, int gridId)
        {
            try
            {
                var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetBySubmissionAndGridAsync(submissionId, gridId);
                if (!rows.Any())
                    return new ApiResponse(404, "No grid rows found");

                foreach (var row in rows)
                {
                    _unitOfWork.FormSubmissionGridRowRepository.Delete(row);
                }

                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, $"{rows.Count()} grid rows deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting grid rows: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Grid row not found");

                entity.IsActive = isActive;
                entity.UpdatedDate = DateTime.UtcNow;
                _unitOfWork.FormSubmissionGridRowRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Grid row {message} successfully", MapToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling grid row active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormSubmissionGridRowRepository.AnyAsync(r => r.Id == id);
                return new ApiResponse(200, "Grid row existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking grid row existence: {ex.Message}");
            }
        }

        public async Task<ApiResponse> RowExistsAsync(int submissionId, int gridId, int rowIndex)
        {
            try
            {
                var exists = await _unitOfWork.FormSubmissionGridRowRepository
                    .RowExistsAsync(submissionId, gridId, rowIndex);
                return new ApiResponse(200, "Row existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking row existence: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetNextRowIndexAsync(int submissionId, int gridId)
        {
            try
            {
                var nextIndex = await _unitOfWork.FormSubmissionGridRowRepository
                    .GetNextRowIndexAsync(submissionId, gridId);
                return new ApiResponse(200, "Next row index retrieved successfully", nextIndex);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error getting next row index: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetRowCountBySubmissionAsync(int submissionId)
        {
            try
            {
                var count = await _unitOfWork.FormSubmissionGridRowRepository
                    .GetRowCountBySubmissionAsync(submissionId);
                return new ApiResponse(200, "Row count retrieved successfully", count);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error getting row count: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetRowCountByGridAsync(int gridId)
        {
            try
            {
                var count = await _unitOfWork.FormSubmissionGridRowRepository
                    .GetRowCountByGridAsync(gridId);
                return new ApiResponse(200, "Row count retrieved successfully", count);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error getting row count: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            try
            {
                var rows = await _unitOfWork.FormSubmissionGridRowRepository
                    .GetByFormBuilderIdAsync(formBuilderId);
                var rowDtos = rows.Select(MapToDto).ToList();
                return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving grid rows: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ReorderRowsAsync(int submissionId, int gridId)
        {
            try
            {
                var rows = await _unitOfWork.FormSubmissionGridRowRepository
                    .GetBySubmissionAndGridAsync(submissionId, gridId);

                var orderedRows = rows.OrderBy(r => r.RowIndex).ToList();

                for (int i = 0; i < orderedRows.Count; i++)
                {
                    orderedRows[i].RowIndex = i;
                    orderedRows[i].UpdatedDate = DateTime.UtcNow;
                    _unitOfWork.FormSubmissionGridRowRepository.Update(orderedRows[i]);
                }

                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Rows reordered successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error reordering rows: {ex.Message}");
            }
        }

        private FormSubmissionGridRowDto MapToDto(FORM_SUBMISSION_GRID_ROWS entity)
        {
            if (entity == null) return null;

            return new FormSubmissionGridRowDto
            {
                Id = entity.Id,
                SubmissionId = entity.SubmissionId,
                SubmissionNumber = entity.FORM_SUBMISSIONS?.DocumentNumber ?? string.Empty,
                GridId = entity.GridId,
                GridName = entity.FORM_GRIDS?.GridName ?? string.Empty,
                RowIndex = entity.RowIndex,
                IsActive = entity.IsActive,
                CreatedDate = entity.CreatedDate,
                UpdatedDate = entity.UpdatedDate
            };
        }
    }
}