using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Services.Services.Base;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FieldDataSourcesService : BaseService<FIELD_DATA_SOURCES, FieldDataSourceDto, CreateFieldDataSourceDto, UpdateFieldDataSourceDto>, IFieldDataSourcesService
    {
        private readonly IunitOfwork _unitOfWork;
        private readonly IFieldDataSourcesRepository _fieldDataSourcesRepository;

        public FieldDataSourcesService(IunitOfwork unitOfWork, IFieldDataSourcesRepository fieldDataSourcesRepository, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _fieldDataSourcesRepository = fieldDataSourcesRepository ?? throw new ArgumentNullException(nameof(fieldDataSourcesRepository));
        }

        protected override IBaseRepository<FIELD_DATA_SOURCES> Repository => _unitOfWork.FieldDataSourcesRepository;

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

        public async Task<ApiResponse> GetByFieldIdAsync(int fieldId)
        {
            var dataSources = await _fieldDataSourcesRepository.GetByFieldIdAsync(fieldId);
            var dataSourceDtos = _mapper.Map<IEnumerable<FieldDataSourceDto>>(dataSources);
            return new ApiResponse(200, "Field data sources retrieved successfully", dataSourceDtos);
        }

        public async Task<ApiResponse> GetActiveByFieldIdAsync(int fieldId)
        {
            var dataSources = await _fieldDataSourcesRepository.GetActiveByFieldIdAsync(fieldId);
            var dataSourceDtos = _mapper.Map<IEnumerable<FieldDataSourceDto>>(dataSources);
            return new ApiResponse(200, "Active field data sources retrieved successfully", dataSourceDtos);
        }

        public async Task<ApiResponse> CreateAsync(CreateFieldDataSourceDto createDto)
        {
            var result = await base.CreateAsync(createDto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFieldDataSourceDto dto)
        {
            // Validate if field exists
            var fieldExists = await _unitOfWork.FormFieldRepository.AnyAsync(x => x.Id == dto.FieldId);
            if (!fieldExists)
                return ValidationResult.Failure("Invalid field ID");

            return ValidationResult.Success();
        }

        public async Task<ApiResponse> CreateBulkAsync(List<CreateFieldDataSourceDto> createDtos)
        {
            if (createDtos == null || !createDtos.Any())
                return new ApiResponse(400, "No field data sources provided");

            // Validate all field IDs exist
            var fieldIds = createDtos.Select(d => d.FieldId).Distinct().ToList();
            foreach (var fieldId in fieldIds)
            {
                var fieldExists = await _unitOfWork.FormFieldRepository.AnyAsync(f => f.Id == fieldId);
                if (!fieldExists)
                    return new ApiResponse(400, $"Invalid field ID: {fieldId}");
            }

            // Validate each DTO
            foreach (var dto in createDtos)
            {
                var validation = await ValidateCreateAsync(dto);
                if (!validation.IsValid)
                    return new ApiResponse(400, validation.ErrorMessage ?? "Validation failed");
            }

            var entities = _mapper.Map<List<FIELD_DATA_SOURCES>>(createDtos);
            foreach (var entity in entities)
            {
                entity.CreatedDate = entity.CreatedDate == default ? DateTime.UtcNow : entity.CreatedDate;
                entity.IsActive = true;
            }

            _unitOfWork.FieldDataSourcesRepository.AddRange(entities);
            await _unitOfWork.CompleteAsyn();

            var resultDtos = _mapper.Map<IEnumerable<FieldDataSourceDto>>(entities);
            return new ApiResponse(200, "Field data sources created successfully", resultDtos);
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFieldDataSourceDto updateDto)
        {
            var result = await base.UpdateAsync(id, updateDto);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            var result = await base.SoftDeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetByFieldIdAndTypeAsync(int fieldId, string sourceType)
        {
            var dataSource = await _fieldDataSourcesRepository.GetByFieldIdAsync(fieldId, sourceType);
            if (dataSource == null)
                return new ApiResponse(404, "Field data source not found for the specified type");

            var dataSourceDto = _mapper.Map<FieldDataSourceDto>(dataSource);
            return new ApiResponse(200, "Field data source retrieved successfully", dataSourceDto);
        }

        public async Task<ApiResponse> GetDataSourcesCountAsync(int fieldId)
        {
            var count = await _fieldDataSourcesRepository.GetDataSourcesCountAsync(fieldId);
            return new ApiResponse(200, "Data sources count retrieved successfully", count);
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
