using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domian.Entitys.froms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FieldDataSourcesService : IFieldDataSourcesService
    {
        private readonly IunitOfwork _unitOfWork;
        private readonly IFieldDataSourcesRepository _fieldDataSourcesRepository;

        public FieldDataSourcesService(IunitOfwork unitOfWork, IFieldDataSourcesRepository fieldDataSourcesRepository)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _fieldDataSourcesRepository = fieldDataSourcesRepository;
        }
        // ================================
        // GET ALL FIELD DATA SOURCES
        // ================================
        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var dataSources = await _unitOfWork.Repositary<FIELD_DATA_SOURCES>().GetAllAsync();
                var dataSourceDtos = dataSources.Select(ToDto).ToList();
                return new ApiResponse(200, "All field data sources retrieved successfully", dataSourceDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all field data sources: {ex.Message}");
            }
        }
        // ================================
        // GET BY FIELD ID
        // ================================
        public async Task<ApiResponse> GetByFieldIdAsync(int fieldId)
        {
            try
            {
                var dataSources = await _fieldDataSourcesRepository.GetByFieldIdAsync(fieldId);
                var dataSourceDtos = dataSources.Select(ToDto).ToList();
                return new ApiResponse(200, "Field data sources retrieved successfully", dataSourceDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving field data sources: {ex.Message}");
            }
        }

        // ================================
        // GET ACTIVE BY FIELD ID
        // ================================
        public async Task<ApiResponse> GetActiveByFieldIdAsync(int fieldId)
        {
            try
            {
                var dataSources = await _fieldDataSourcesRepository.GetActiveByFieldIdAsync(fieldId);
                var dataSourceDtos = dataSources.Select(ToDto).ToList();
                return new ApiResponse(200, "Active field data sources retrieved successfully", dataSourceDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active field data sources: {ex.Message}");
            }
        }

        // ================================
        // GET BY ID
        // ================================
        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var dataSource = await _fieldDataSourcesRepository.GetByIdAsync(id);
                if (dataSource == null)
                    return new ApiResponse(404, "Field data source not found");

                var dataSourceDto = ToDto(dataSource);
                return new ApiResponse(200, "Field data source retrieved successfully", dataSourceDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving field data source: {ex.Message}");
            }
        }

        // ================================
        // CREATE
        // ================================
        public async Task<ApiResponse> CreateAsync(CreateFieldDataSourceDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Validate if field exists
                var fieldExists = await _unitOfWork.Repositary<FORM_FIELDS>().AnyAsync(x => x.id == createDto.FieldId);
                if (!fieldExists)
                    return new ApiResponse(400, "Invalid field ID");

                var entity = ToEntity(createDto);
                _unitOfWork.Repositary<FIELD_DATA_SOURCES>().Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Field data source created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating field data source: {ex.Message}");
            }
        }

        // ================================
        // CREATE BULK
        // ================================
        public async Task<ApiResponse> CreateBulkAsync(List<CreateFieldDataSourceDto> createDtos)
        {
            try
            {
                if (createDtos == null || !createDtos.Any())
                    return new ApiResponse(400, "No field data sources provided");

                var entities = createDtos.Select(ToEntity).ToList();
                _unitOfWork.Repositary<FIELD_DATA_SOURCES>().AddRange(entities);
                await _unitOfWork.CompleteAsyn();

                var resultDtos = entities.Select(ToDto).ToList();
                return new ApiResponse(200, "Field data sources created successfully", resultDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating field data sources: {ex.Message}");
            }
        }

        // ================================
        // UPDATE
        // ================================
        public async Task<ApiResponse> UpdateAsync(int id, UpdateFieldDataSourceDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _fieldDataSourcesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Field data source not found");

                MapUpdate(updateDto, entity);
                _unitOfWork.Repositary<FIELD_DATA_SOURCES>().Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Field data source updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating field data source: {ex.Message}");
            }
        }

        // ================================
        // DELETE (HARD)
        // ================================
        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _fieldDataSourcesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Field data source not found");

                _unitOfWork.Repositary<FIELD_DATA_SOURCES>().Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Field data source deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting field data source: {ex.Message}");
            }
        }

        // ================================
        // SOFT DELETE
        // ================================
        public async Task<ApiResponse> SoftDeleteAsync(int id)
        {
            try
            {
                var entity = await _fieldDataSourcesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Field data source not found");

                entity.IsActive = false;
                _unitOfWork.Repositary<FIELD_DATA_SOURCES>().Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Field data source soft deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error soft deleting field data source: {ex.Message}");
            }
        }

        // ================================
        // GET BY FIELD ID AND TYPE
        // ================================
        public async Task<ApiResponse> GetByFieldIdAndTypeAsync(int fieldId, string sourceType)
        {
            try
            {
                var dataSource = await _fieldDataSourcesRepository.GetByFieldIdAsync(fieldId, sourceType);
                if (dataSource == null)
                    return new ApiResponse(404, "Field data source not found for the specified type");

                var dataSourceDto = ToDto(dataSource);
                return new ApiResponse(200, "Field data source retrieved successfully", dataSourceDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving field data source: {ex.Message}");
            }
        }

        // ================================
        // GET DATA SOURCES COUNT
        // ================================
        public async Task<ApiResponse> GetDataSourcesCountAsync(int fieldId)
        {
            try
            {
                var count = await _fieldDataSourcesRepository.GetDataSourcesCountAsync(fieldId);
                return new ApiResponse(200, "Data sources count retrieved successfully", count);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving data sources count: {ex.Message}");
            }
        }

        // ================================
        // MAPPING
        // ================================
        private FieldDataSourceDto ToDto(FIELD_DATA_SOURCES entity)
        {
            if (entity == null) return null;

            return new FieldDataSourceDto
            {
                Id = entity.id,
                FieldId = entity.FieldId,
                SourceType = entity.SourceType,
                ApiUrl = entity.ApiUrl,
                HttpMethod = entity.HttpMethod,
                RequestBodyJson = entity.RequestBodyJson,
                ValuePath = entity.ValuePath,
                TextPath = entity.TextPath,
                IsActive = entity.IsActive
            };
        }

        private FIELD_DATA_SOURCES ToEntity(CreateFieldDataSourceDto dto)
        {
            return new FIELD_DATA_SOURCES
            {
                FieldId = dto.FieldId,
                SourceType = dto.SourceType,
                ApiUrl = dto.ApiUrl,
                HttpMethod = dto.HttpMethod,
                RequestBodyJson = dto.RequestBodyJson,
                ValuePath = dto.ValuePath,
                TextPath = dto.TextPath,
                IsActive = dto.IsActive
            };
        }

        private void MapUpdate(UpdateFieldDataSourceDto dto, FIELD_DATA_SOURCES entity)
        {
            entity.SourceType = dto.SourceType;
            entity.ApiUrl = dto.ApiUrl;
            entity.HttpMethod = dto.HttpMethod;
            entity.RequestBodyJson = dto.RequestBodyJson;
            entity.ValuePath = dto.ValuePath;
            entity.TextPath = dto.TextPath;
            entity.IsActive = dto.IsActive;
        }
    }
}