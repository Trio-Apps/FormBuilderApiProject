using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.FormBuilder;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using FormBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
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
    public class DocumentSeriesService : BaseService<DOCUMENT_SERIES, DocumentSeriesDto, CreateDocumentSeriesDto, UpdateDocumentSeriesDto>, IDocumentSeriesService
    {
        private readonly IunitOfwork _unitOfWork;

        public DocumentSeriesService(IunitOfwork unitOfWork, IMapper mapper) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        protected override IBaseRepository<DOCUMENT_SERIES> Repository => _unitOfWork.DocumentSeriesRepository;

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

        public async Task<ApiResponse> GetBySeriesCodeAsync(string seriesCode)
        {
            var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetBySeriesCodeAsync(seriesCode);
            if (documentSeries == null)
                return new ApiResponse(404, "Document series not found");

            var documentSeriesDto = _mapper.Map<DocumentSeriesDto>(documentSeries);
            return new ApiResponse(200, "Document series retrieved successfully", documentSeriesDto);
        }

        public async Task<ApiResponse> GetByDocumentTypeIdAsync(int documentTypeId)
        {
            var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetByDocumentTypeIdAsync(documentTypeId);
            var documentSeriesDtos = _mapper.Map<IEnumerable<DocumentSeriesDto>>(documentSeries);
            return new ApiResponse(200, "Document series retrieved successfully", documentSeriesDtos);
        }

        public async Task<ApiResponse> GetByProjectIdAsync(int projectId)
        {
            var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetByProjectIdAsync(projectId);
            var documentSeriesDtos = _mapper.Map<IEnumerable<DocumentSeriesDto>>(documentSeries);
            return new ApiResponse(200, "Document series retrieved successfully", documentSeriesDtos);
        }

        public async Task<ApiResponse> GetActiveAsync()
        {
            var result = await base.GetActiveAsync();
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> GetDefaultSeriesAsync(int documentTypeId, int projectId)
        {
            var documentSeries = await _unitOfWork.DocumentSeriesRepository.GetDefaultSeriesAsync(documentTypeId, projectId);
            if (documentSeries == null)
                return new ApiResponse(404, "Default document series not found");

            var documentSeriesDto = _mapper.Map<DocumentSeriesDto>(documentSeries);
            return new ApiResponse(200, "Default document series retrieved successfully", documentSeriesDto);
        }

        public async Task<ApiResponse> CreateAsync(CreateDocumentSeriesDto createDto)
        {
            // If setting as default, remove default from other series with same document type and project
            if (createDto.IsDefault)
            {
                await RemoveDefaultFromOtherSeriesAsync(createDto.DocumentTypeId, createDto.ProjectId);
            }

            var result = await base.CreateAsync(createDto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateDocumentSeriesDto dto)
        {
            var codeExists = await _unitOfWork.DocumentSeriesRepository.SeriesCodeExistsAsync(dto.SeriesCode);
            if (codeExists)
                return ValidationResult.Failure("Document series code already exists");

            return ValidationResult.Success();
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateDocumentSeriesDto updateDto)
        {
            var entity = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(id);
            if (entity == null)
                return new ApiResponse(404, "Document series not found");

            // If setting as default, remove default from other series with same document type and project
            if (updateDto.IsDefault.HasValue && updateDto.IsDefault.Value)
            {
                var documentTypeId = updateDto.DocumentTypeId ?? entity.DocumentTypeId;
                var projectId = updateDto.ProjectId ?? entity.ProjectId;
                await RemoveDefaultFromOtherSeriesAsync(documentTypeId, projectId, id);
            }

            var result = await base.UpdateAsync(id, updateDto);
            return ConvertToApiResponse(result);
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateDocumentSeriesDto dto, DOCUMENT_SERIES entity)
        {
            // Check if series code already exists (excluding current record)
            if (!string.IsNullOrEmpty(dto.SeriesCode) && dto.SeriesCode != entity.SeriesCode)
            {
                var codeExists = await _unitOfWork.DocumentSeriesRepository.SeriesCodeExistsAsync(dto.SeriesCode, id);
                if (codeExists)
                    return ValidationResult.Failure("Document series code already exists");
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

        public async Task<ApiResponse> SetAsDefaultAsync(int id)
        {
            var entity = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(id);
            if (entity == null)
                return new ApiResponse(404, "Document series not found");

            // Remove default from other series with same document type and project
            await RemoveDefaultFromOtherSeriesAsync(entity.DocumentTypeId, entity.ProjectId, id);

            entity.IsDefault = true;
            _unitOfWork.DocumentSeriesRepository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            var dto = _mapper.Map<DocumentSeriesDto>(entity);
            return new ApiResponse(200, "Document series set as default successfully", dto);
        }

        public async Task<ApiResponse> GetNextNumberAsync(int seriesId)
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

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            var exists = await _unitOfWork.DocumentSeriesRepository.AnyAsync(s => s.Id == id);
            return new ApiResponse(200, "Document series existence checked successfully", exists);
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