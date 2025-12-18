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
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormGridColumnService : BaseService<FORM_GRID_COLUMNS, FormGridColumnDto, CreateFormGridColumnDto, UpdateFormGridColumnDto>, IFormGridColumnService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormGridColumnService(IunitOfwork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override IBaseRepository<FORM_GRID_COLUMNS> Repository => _unitOfWork.FormGridColumnRepository;

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

        public async Task<ApiResponse> GetByGridIdAsync(int gridId)
        {
            var columns = await _unitOfWork.FormGridColumnRepository.GetByGridIdAsync(gridId);
            var columnDtos = _mapper.Map<IEnumerable<FormGridColumnDto>>(columns);
            return new ApiResponse(200, "Grid columns by grid retrieved successfully", columnDtos);
        }

        public async Task<ApiResponse> GetActiveByGridIdAsync(int gridId)
        {
            var columns = await _unitOfWork.FormGridColumnRepository.GetActiveByGridIdAsync(gridId);
            var columnDtos = _mapper.Map<IEnumerable<FormGridColumnDto>>(columns);
            return new ApiResponse(200, "Active grid columns retrieved successfully", columnDtos);
        }

        public async Task<ApiResponse> GetByColumnCodeAsync(string columnCode, int gridId)
        {
            var column = await _unitOfWork.FormGridColumnRepository.GetByColumnCodeAsync(columnCode, gridId);
            if (column == null)
                return new ApiResponse(404, "Grid column not found");

            var columnDto = _mapper.Map<FormGridColumnDto>(column);
            return new ApiResponse(200, "Grid column retrieved successfully", columnDto);
        }

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            var columns = await _unitOfWork.FormGridColumnRepository.GetByFormBuilderIdAsync(formBuilderId);
            var columnDtos = _mapper.Map<IEnumerable<FormGridColumnDto>>(columns);
            return new ApiResponse(200, "Grid columns by form builder retrieved successfully", columnDtos);
        }

        public async Task<ApiResponse> GetByFieldTypeIdAsync(int fieldTypeId)
        {
            var columns = await _unitOfWork.FormGridColumnRepository.GetByFieldTypeIdAsync(fieldTypeId);
            var columnDtos = _mapper.Map<IEnumerable<FormGridColumnDto>>(columns);
            return new ApiResponse(200, "Grid columns by field type retrieved successfully", columnDtos);
        }

        public async Task<ApiResponse> CreateAsync(CreateFormGridColumnDto createDto)
        {
            // Get next column order if not specified
            if (!createDto.ColumnOrder.HasValue)
            {
                createDto.ColumnOrder = await _unitOfWork.FormGridColumnRepository.GetNextColumnOrderAsync(createDto.GridId);
            }

            var result = await base.CreateAsync(createDto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFormGridColumnDto dto)
        {
            var codeExists = await _unitOfWork.FormGridColumnRepository.ColumnCodeExistsAsync(dto.ColumnCode, dto.GridId);
            if (codeExists)
                return ValidationResult.Failure("Grid column code already exists for this grid");

            return ValidationResult.Success();
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormGridColumnDto updateDto)
        {
            var result = await base.UpdateAsync(id, updateDto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateFormGridColumnDto dto, FORM_GRID_COLUMNS entity)
        {
            // Check if column code already exists (excluding current record)
            if (!string.IsNullOrEmpty(dto.ColumnCode) && dto.ColumnCode != entity.ColumnCode)
            {
                var codeExists = await _unitOfWork.FormGridColumnRepository.ColumnCodeExistsAsync(
                    dto.ColumnCode, entity.GridId, id);

                if (codeExists)
                    return ValidationResult.Failure("Grid column code already exists for this grid");
            }

            return ValidationResult.Success();
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            var result = await base.ToggleActiveAsync(id, isActive);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            var exists = await _unitOfWork.FormGridColumnRepository.AnyAsync(c => c.Id == id);
            return new ApiResponse(200, "Grid column existence checked successfully", exists);
        }

        public async Task<ApiResponse> ColumnCodeExistsAsync(string columnCode, int gridId, int? excludeId = null)
        {
            var exists = await _unitOfWork.FormGridColumnRepository.ColumnCodeExistsAsync(
                columnCode, gridId, excludeId);

            return new ApiResponse(200, "Grid column code existence checked successfully", exists);
        }

        public async Task<ApiResponse> GetNextColumnOrderAsync(int gridId)
        {
            var nextOrder = await _unitOfWork.FormGridColumnRepository.GetNextColumnOrderAsync(gridId);
            return new ApiResponse(200, "Next column order retrieved successfully", nextOrder);
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
