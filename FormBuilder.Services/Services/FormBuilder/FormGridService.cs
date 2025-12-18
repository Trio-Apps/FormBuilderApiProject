using formBuilder.Domian.Interfaces;
using FormBuilder.API.DTOs;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
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
    public class FormGridService : BaseService<FORM_GRIDS, FormGridDto, CreateFormGridDto, UpdateFormGridDto>, IFormGridService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormGridService(IunitOfwork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override IBaseRepository<FORM_GRIDS> Repository => _unitOfWork.FormGridRepository;

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

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            var formGrids = await _unitOfWork.FormGridRepository.GetByFormBuilderIdAsync(formBuilderId);
            var formGridDtos = _mapper.Map<IEnumerable<FormGridDto>>(formGrids);
            return new ApiResponse(200, "Form grids by form builder retrieved successfully", formGridDtos);
        }

        public async Task<ApiResponse> GetByTabIdAsync(int tabId)
        {
            var formGrids = await _unitOfWork.FormGridRepository.GetByTabIdAsync(tabId);
            var formGridDtos = _mapper.Map<IEnumerable<FormGridDto>>(formGrids);
            return new ApiResponse(200, "Form grids by tab retrieved successfully", formGridDtos);
        }

        public async Task<ApiResponse> GetActiveByFormBuilderIdAsync(int formBuilderId)
        {
            var formGrids = await _unitOfWork.FormGridRepository.GetActiveByFormBuilderIdAsync(formBuilderId);
            var formGridDtos = _mapper.Map<IEnumerable<FormGridDto>>(formGrids);
            return new ApiResponse(200, "Active form grids by form builder retrieved successfully", formGridDtos);
        }

        public async Task<ApiResponse> GetByGridCodeAsync(string gridCode, int formBuilderId)
        {
            var formGrid = await _unitOfWork.FormGridRepository.GetByGridCodeAsync(gridCode, formBuilderId);
            if (formGrid == null)
                return new ApiResponse(404, "Form grid not found");

            var formGridDto = _mapper.Map<FormGridDto>(formGrid);
            return new ApiResponse(200, "Form grid retrieved successfully", formGridDto);
        }

        public async Task<ApiResponse> CreateAsync(CreateFormGridDto createDto)
        {
            // Get next grid order if not specified
            var gridOrder = createDto.GridOrder ??
                await _unitOfWork.FormGridRepository.GetNextGridOrderAsync(createDto.FormBuilderId, createDto.TabId);

            var entity = _mapper.Map<FORM_GRIDS>(createDto);
            entity.GridOrder = gridOrder;
            entity.CreatedDate = DateTime.UtcNow;
            entity.IsActive = true;

            Repository.Add(entity);
            await _unitOfWork.CompleteAsyn();

            var dto = _mapper.Map<FormGridDto>(entity);
            return new ApiResponse(200, "Form grid created successfully", dto);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFormGridDto dto)
        {
            var codeExists = await _unitOfWork.FormGridRepository.GridCodeExistsAsync(dto.GridCode, dto.FormBuilderId);
            if (codeExists)
                return ValidationResult.Failure("Form grid code already exists for this form builder");

            return ValidationResult.Success();
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormGridDto updateDto)
        {
            var result = await base.UpdateAsync(id, updateDto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateFormGridDto dto, FORM_GRIDS entity)
        {
            if (!string.IsNullOrEmpty(dto.GridCode) && dto.GridCode != entity.GridCode)
            {
                var codeExists = await _unitOfWork.FormGridRepository.GridCodeExistsAsync(dto.GridCode, entity.FormBuilderId, id);
                if (codeExists)
                    return ValidationResult.Failure("Form grid code already exists for this form builder");
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
            var exists = await _unitOfWork.FormGridRepository.AnyAsync(g => g.Id == id);
            return new ApiResponse(200, "Form grid existence checked successfully", exists);
        }

        public async Task<ApiResponse> GridCodeExistsAsync(string gridCode, int formBuilderId, int? excludeId = null)
        {
            var exists = await _unitOfWork.FormGridRepository.GridCodeExistsAsync(gridCode, formBuilderId, excludeId);
            return new ApiResponse(200, "Form grid code existence checked successfully", exists);
        }

        public async Task<ApiResponse> GetNextGridOrderAsync(int formBuilderId, int? tabId = null)
        {
            var nextOrder = await _unitOfWork.FormGridRepository.GetNextGridOrderAsync(formBuilderId, tabId);
            return new ApiResponse(200, "Next grid order retrieved successfully", nextOrder);
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
    }
}