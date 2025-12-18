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
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionGridCellService : BaseService<FORM_SUBMISSION_GRID_CELLS, FormSubmissionGridCellDto, CreateFormSubmissionGridCellDto, UpdateFormSubmissionGridCellDto>, IFormSubmissionGridCellService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormSubmissionGridCellService(IunitOfwork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override IBaseRepository<FORM_SUBMISSION_GRID_CELLS> Repository => _unitOfWork.FormSubmissionGridCellRepository;

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

        public async Task<ApiResponse> GetByRowIdAsync(int rowId)
        {
            var cells = await _unitOfWork.FormSubmissionGridCellRepository.GetByRowIdAsync(rowId);
            var cellDtos = cells.Select(cell =>
            {
                var dto = _mapper.Map<FormSubmissionGridCellDto>(cell);
                dto.DisplayValue = GetDisplayValue(cell);
                return dto;
            }).ToList();
            return new ApiResponse(200, "Grid cells retrieved successfully", cellDtos);
        }

        public async Task<ApiResponse> GetByRowAndColumnAsync(int rowId, int columnId)
        {
            var cell = await _unitOfWork.FormSubmissionGridCellRepository.GetByRowAndColumnAsync(rowId, columnId);
            if (cell == null)
                return new ApiResponse(404, "Grid cell not found");

            var cellDto = _mapper.Map<FormSubmissionGridCellDto>(cell);
            cellDto.DisplayValue = GetDisplayValue(cell);
            return new ApiResponse(200, "Grid cell retrieved successfully", cellDto);
        }

        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionGridCellDto createDto)
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

            var entity = _mapper.Map<FORM_SUBMISSION_GRID_CELLS>(createDto);
            entity.CreatedDate = DateTime.UtcNow;

            _unitOfWork.FormSubmissionGridCellRepository.Add(entity);
            await _unitOfWork.CompleteAsyn();

            var createdEntity = await _unitOfWork.FormSubmissionGridCellRepository.GetByIdAsync(entity.Id);
            var cellDto = _mapper.Map<FormSubmissionGridCellDto>(createdEntity);
            cellDto.DisplayValue = GetDisplayValue(createdEntity);

            return new ApiResponse(201, "Grid cell created successfully", cellDto);
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionGridCellDto updateDto)
        {
            var result = await base.UpdateAsync(id, updateDto);
            if (!result.Success)
                return ConvertToApiResponse(result);

            // Get updated entity to set DisplayValue
            var updatedEntity = await _unitOfWork.FormSubmissionGridCellRepository.GetByIdAsync(id);
            var cellDto = _mapper.Map<FormSubmissionGridCellDto>(updatedEntity);
            cellDto.DisplayValue = GetDisplayValue(updatedEntity);

            return new ApiResponse(200, "Grid cell updated successfully", cellDto);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> DeleteByRowIdAsync(int rowId)
        {
            var row = await _unitOfWork.FormSubmissionGridRowRepository.GetByIdAsync(rowId);
            if (row == null)
                return new ApiResponse(404, "Grid row not found");

            var deletedCount = await _unitOfWork.FormSubmissionGridCellRepository.DeleteByRowIdAsync(rowId);
            await _unitOfWork.CompleteAsyn();

            return new ApiResponse(200, $"{deletedCount} grid cells deleted successfully", deletedCount);
        }

        // ================================
        // HELPER METHODS
        // ================================
        private string GetDisplayValue(FORM_SUBMISSION_GRID_CELLS cell)
        {
            if (cell == null) return string.Empty;

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
