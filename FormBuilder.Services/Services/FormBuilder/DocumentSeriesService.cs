using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class DocumentSeriesService : IDocumentSeriesService
    {
        private readonly IunitOfwork _unitOfWork;

        public DocumentSeriesService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetAllAsync();
                var documentSeriesDtos = documentSeries.Select(ToDto).ToList();
                return new ApiResponse(200, "All document series retrieved successfully", documentSeriesDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving all document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(id);
                if (documentSeries == null)
                    return new ApiResponse(404, "Document series not found");

                var documentSeriesDto = ToDto(documentSeries);
                return new ApiResponse(200, "Document series retrieved successfully", documentSeriesDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetBySeriesCodeAsync(string seriesCode)
        {
            try
            {
                var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetBySeriesCodeAsync(seriesCode);
                if (documentSeries == null)
                    return new ApiResponse(404, "Document series not found");

                var documentSeriesDto = ToDto(documentSeries);
                return new ApiResponse(200, "Document series retrieved successfully", documentSeriesDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByDocumentTypeIdAsync(int documentTypeId)
        {
            try
            {
                var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetByDocumentTypeIdAsync(documentTypeId);
                var documentSeriesDtos = documentSeries.Select(ToDto).ToList();
                return new ApiResponse(200, "Document series retrieved successfully", documentSeriesDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByProjectIdAsync(int projectId)
        {
            try
            {
                var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetByProjectIdAsync(projectId);
                var documentSeriesDtos = documentSeries.Select(ToDto).ToList();
                return new ApiResponse(200, "Document series retrieved successfully", documentSeriesDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetActiveAsync()
        {
            try
            {
                var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetActiveAsync();
                var documentSeriesDtos = documentSeries.Select(ToDto).ToList();
                return new ApiResponse(200, "Active document series retrieved successfully", documentSeriesDtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving active document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetDefaultSeriesAsync(int documentTypeId, int projectId)
        {
            try
            {
                var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetDefaultSeriesAsync(documentTypeId, projectId);
                if (documentSeries == null)
                    return new ApiResponse(404, "Default document series not found");

                var documentSeriesDto = ToDto(documentSeries);
                return new ApiResponse(200, "Default document series retrieved successfully", documentSeriesDto);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving default document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(CreateDocumentSeriesDto createDto)
        {
            try
            {
                if (createDto == null)
                    return new ApiResponse(400, "DTO is required");

                // Check if series code already exists
                var codeExists = await _unitOfWork.DocumentSeriesRepository.SeriesCodeExistsAsync(createDto.SeriesCode);
                if (codeExists)
                    return new ApiResponse(400, "Document series code already exists");

                // If setting as default, remove default from other series with same document type and project
                if (createDto.IsDefault)
                {
                    await RemoveDefaultFromOtherSeriesAsync(createDto.DocumentTypeId, createDto.ProjectId);
                }

                var entity = ToEntity(createDto);
                _unitOfWork.DocumentSeriesRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Document series created successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error creating document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateDocumentSeriesDto updateDto)
        {
            try
            {
                if (updateDto == null)
                    return new ApiResponse(400, "DTO is required");

                var entity = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Document series not found");

                // Check if series code already exists (excluding current record)
                if (!string.IsNullOrEmpty(updateDto.SeriesCode) && updateDto.SeriesCode != entity.SeriesCode)
                {
                    var codeExists = await _unitOfWork.DocumentSeriesRepository.SeriesCodeExistsAsync(updateDto.SeriesCode, id);
                    if (codeExists)
                        return new ApiResponse(400, "Document series code already exists");
                }

                // If setting as default, remove default from other series with same document type and project
                if (updateDto.IsDefault.HasValue && updateDto.IsDefault.Value)
                {
                    var documentTypeId = updateDto.DocumentTypeId ?? entity.DocumentTypeId;
                    var projectId = updateDto.ProjectId ?? entity.ProjectId;
                    await RemoveDefaultFromOtherSeriesAsync(documentTypeId, projectId, id);
                }

                MapUpdate(updateDto, entity);
                _unitOfWork.DocumentSeriesRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Document series updated successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error updating document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Document series not found");

                _unitOfWork.DocumentSeriesRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Document series deleted successfully");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error deleting document series: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ToggleActiveAsync(int id, bool isActive)
        {
            try
            {
                var entity = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Document series not found");

                entity.IsActive = isActive;
                _unitOfWork.DocumentSeriesRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                var message = isActive ? "activated" : "deactivated";
                return new ApiResponse(200, $"Document series {message} successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error toggling document series active status: {ex.Message}");
            }
        }

        public async Task<ApiResponse> SetAsDefaultAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Document series not found");

                // Remove default from other series with same document type and project
                await RemoveDefaultFromOtherSeriesAsync(entity.DocumentTypeId, entity.ProjectId, id);

                entity.IsDefault = true;
                _unitOfWork.DocumentSeriesRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Document series set as default successfully", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error setting document series as default: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetNextNumberAsync(int seriesId)
        {
            try
            {
                var nextNumber = await _unitOfWork.DocumentSeriesRepository.GetNextNumberAsync(seriesId);
                if (nextNumber == -1)
                    return new ApiResponse(404, "Document series not found");

                var series = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(seriesId);
                var result = new DocumentSeriesNumberDto
                {
                    SeriesId = seriesId,
                    SeriesCode = series.SeriesCode,
                    NextNumber = nextNumber,
                    FullNumber = $"{series.SeriesCode}-{nextNumber:D6}"
                };

                return new ApiResponse(200, "Next number retrieved successfully", result);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error retrieving next number: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.DocumentSeriesRepository.AnyAsync(s => s.Id == id);
                return new ApiResponse(200, "Document series existence checked successfully", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error checking document series existence: {ex.Message}");
            }
        }

        // ================================
        // PRIVATE HELPER METHODS
        // ================================
        private async Task RemoveDefaultFromOtherSeriesAsync(int documentTypeId, int projectId, int? excludeId = null)
        {
            var existingDefaultSeries = await _unitOfWork.DocumentSeriesRepository
                .GetDefaultSeriesAsync(documentTypeId, projectId);

            if (existingDefaultSeries != null && existingDefaultSeries.Id != excludeId)
            {
                existingDefaultSeries.IsDefault = false;
                _unitOfWork.DocumentSeriesRepository.Update(existingDefaultSeries);
            }
        }

        // ================================
        // MAPPING METHODS
        // ================================
        private DocumentSeriesDto ToDto(DOCUMENT_SERIES entity)
        {
            if (entity == null) return null;

            return new DocumentSeriesDto
            {
                Id = entity.Id,
                DocumentTypeId = entity.DocumentTypeId,
                DocumentTypeName = entity.DOCUMENT_TYPES?.Name,
                ProjectId = entity.ProjectId,
                ProjectName = entity.PROJECTS?.Name,
                SeriesCode = entity.SeriesCode,
                NextNumber = entity.NextNumber,
                IsDefault = entity.IsDefault,
                IsActive = entity.IsActive
            };
        }

        private DOCUMENT_SERIES ToEntity(CreateDocumentSeriesDto dto)
        {
            return new DOCUMENT_SERIES
            {
                DocumentTypeId = dto.DocumentTypeId,
                ProjectId = dto.ProjectId,
                SeriesCode = dto.SeriesCode,
                NextNumber = dto.NextNumber,
                IsDefault = dto.IsDefault,
                IsActive = dto.IsActive
            };
        }

        private void MapUpdate(UpdateDocumentSeriesDto dto, DOCUMENT_SERIES entity)
        {
            if (dto.DocumentTypeId.HasValue)
                entity.DocumentTypeId = dto.DocumentTypeId.Value;

            if (dto.ProjectId.HasValue)
                entity.ProjectId = dto.ProjectId.Value;

            if (!string.IsNullOrEmpty(dto.SeriesCode))
                entity.SeriesCode = dto.SeriesCode;

            if (dto.NextNumber.HasValue)
                entity.NextNumber = dto.NextNumber.Value;

            if (dto.IsDefault.HasValue)
                entity.IsDefault = dto.IsDefault.Value;

            if (dto.IsActive.HasValue)
                entity.IsActive = dto.IsActive.Value;
        }
    }
}