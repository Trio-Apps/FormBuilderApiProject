using formBuilder.Domian.Interfaces;
using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Services.Services.Base;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionGridRowService : BaseService<FORM_SUBMISSION_GRID_ROWS, FormSubmissionGridRowDto, CreateFormSubmissionGridRowDto, UpdateFormSubmissionGridRowDto>, IFormSubmissionGridRowService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionGridRowService(IunitOfwork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override IBaseRepository<FORM_SUBMISSION_GRID_ROWS> Repository => _unitOfWork.FormSubmissionGridRowRepository;

        public async Task<ApiResponse> GetAllAsync()
        {
            var result = await base.GetAllAsync();
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var result = await base.GetByIdAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetBySubmissionIdAsync(int submissionId)
        {
            var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetBySubmissionIdAsync(submissionId);
            var rowDtos = _mapper.Map<IEnumerable<FormSubmissionGridRowDto>>(rows);
            return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
        }

        public async Task<ApiResponse> GetByGridIdAsync(int gridId)
        {
            var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetByGridIdAsync(gridId);
            var rowDtos = _mapper.Map<IEnumerable<FormSubmissionGridRowDto>>(rows);
            return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
        }

        public async Task<ApiResponse> GetBySubmissionAndGridAsync(int submissionId, int gridId)
        {
            var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetBySubmissionAndGridAsync(submissionId, gridId);
            var rowDtos = _mapper.Map<IEnumerable<FormSubmissionGridRowDto>>(rows);
            return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
        }

        public async Task<ApiResponse> GetActiveRowsAsync(int submissionId, int gridId)
        {
            var rows = await _unitOfWork.FormSubmissionGridRowRepository.GetActiveRowsAsync(submissionId, gridId);
            var rowDtos = _mapper.Map<IEnumerable<FormSubmissionGridRowDto>>(rows);
            return new ApiResponse(200, "Active grid rows retrieved successfully", rowDtos);
        }

        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionGridRowDto createDto)
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

            var entity = _mapper.Map<FORM_SUBMISSION_GRID_ROWS>(createDto);
            entity.RowIndex = rowIndex;
            entity.CreatedDate = DateTime.UtcNow;

            _unitOfWork.FormSubmissionGridRowRepository.Add(entity);
            await _unitOfWork.CompleteAsyn();

            var createdEntity = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(entity.Id);
            var rowDto = _mapper.Map<FormSubmissionGridRowDto>(createdEntity);

            return new ApiResponse(201, "Grid row created successfully", rowDto);
        }

        public async Task<ApiResponse> CreateMultipleAsync(List<CreateFormSubmissionGridRowDto> createDtos)
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

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionGridRowDto updateDto)
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

            _mapper.Map(updateDto, entity);
            entity.UpdatedDate = DateTime.UtcNow;
            _unitOfWork.FormSubmissionGridRowRepository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            var updatedEntity = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(id);
            var rowDto = _mapper.Map<FormSubmissionGridRowDto>(updatedEntity);

            return new ApiResponse(200, "Grid row updated successfully", rowDto);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> DeleteBySubmissionAndGridAsync(int submissionId, int gridId)
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

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            var result = await base.ToggleActiveAsync(id, isActive);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            var exists = await _unitOfWork.FormSubmissionGridRowRepository.AnyAsync(r => r.Id == id);
            return new ApiResponse(200, "Grid row existence checked successfully", exists);
        }

        public async Task<ApiResponse> RowExistsAsync(int submissionId, int gridId, int rowIndex)
        {
            var exists = await _unitOfWork.FormSubmissionGridRowRepository
                .RowExistsAsync(submissionId, gridId, rowIndex);
            return new ApiResponse(200, "Row existence checked successfully", exists);
        }

        public async Task<ApiResponse> GetNextRowIndexAsync(int submissionId, int gridId)
        {
            var nextIndex = await _unitOfWork.FormSubmissionGridRowRepository
                .GetNextRowIndexAsync(submissionId, gridId);
            return new ApiResponse(200, "Next row index retrieved successfully", nextIndex);
        }

        public async Task<ApiResponse> GetRowCountBySubmissionAsync(int submissionId)
        {
            var count = await _unitOfWork.FormSubmissionGridRowRepository
                .GetRowCountBySubmissionAsync(submissionId);
            return new ApiResponse(200, "Row count retrieved successfully", count);
        }

        public async Task<ApiResponse> GetRowCountByGridAsync(int gridId)
        {
            var count = await _unitOfWork.FormSubmissionGridRowRepository
                .GetRowCountByGridAsync(gridId);
            return new ApiResponse(200, "Row count retrieved successfully", count);
        }

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            var rows = await _unitOfWork.FormSubmissionGridRowRepository
                .GetByFormBuilderIdAsync(formBuilderId);
            var rowDtos = _mapper.Map<IEnumerable<FormSubmissionGridRowDto>>(rows);
            return new ApiResponse(200, "Grid rows retrieved successfully", rowDtos);
        }

        public async Task<ApiResponse> ReorderRowsAsync(int submissionId, int gridId)
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

        // ================================
        // HELPER METHODS
        // ================================
        private ApiResponse ConvertToApiResponse<T>(ServiceResult<T> result)
        {
            if (result.Success)
                return new ApiResponse(result.StatusCode, "Success", result.Data);
            else
                return new ApiResponse(result.StatusCode, result.ErrorMessage);
        }

        private ApiResponse ConvertToApiResponse(ServiceResult<bool> result)
        {
            if (result.Success)
                return new ApiResponse(result.StatusCode, "Success", result.Data);
            else
                return new ApiResponse(result.StatusCode, result.ErrorMessage);
        }
    }
}
