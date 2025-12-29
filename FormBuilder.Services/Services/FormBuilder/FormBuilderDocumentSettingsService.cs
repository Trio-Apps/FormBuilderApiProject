using AutoMapper;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using formBuilder.Domian.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services.FormBuilder
{
    public class FormBuilderDocumentSettingsService : IFormBuilderDocumentSettingsService
    {
        private readonly IunitOfwork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IStringLocalizer<FormBuilderDocumentSettingsService>? _localizer;

        public FormBuilderDocumentSettingsService(
            IunitOfwork unitOfWork,
            IMapper mapper,
            IStringLocalizer<FormBuilderDocumentSettingsService>? localizer = null)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _localizer = localizer;
        }

        public async Task<ServiceResult<DocumentSettingsDto>> GetDocumentSettingsAsync(int formBuilderId)
        {
            try
            {
                // Verify FormBuilder exists
                var formBuilder = await _unitOfWork.FormBuilderRepository.SingleOrDefaultAsync(fb => fb.Id == formBuilderId);
                if (formBuilder == null)
                {
                    var message = _localizer?["FormBuilder_NotFound"] ?? "Form Builder not found";
                    return ServiceResult<DocumentSettingsDto>.NotFound(message);
                }

                // Get Document Type for this FormBuilder
                var documentTypes = await _unitOfWork.DocumentTypeRepository.GetByFormBuilderIdAsync(formBuilderId);
                var documentType = documentTypes.FirstOrDefault();

                var result = new DocumentSettingsDto
                {
                    FormBuilderId = formBuilderId,
                    FormBuilderName = formBuilder.FormName
                };

                if (documentType != null)
                {
                    result.DocumentTypeId = documentType.Id;
                    result.DocumentName = documentType.Name;
                    result.DocumentCode = documentType.Code;
                    result.MenuCaption = documentType.MenuCaption;
                    result.MenuOrder = documentType.MenuOrder;
                    result.ParentMenuId = documentType.ParentMenuId;
                    result.IsActive = documentType.IsActive;

                    // Get all Document Series for this Document Type
                    var seriesList = await _unitOfWork.DocumentSeriesRepository.GetByDocumentTypeIdAsync(documentType.Id);
                    result.DocumentSeries = _mapper.Map<List<DocumentSeriesDto>>(seriesList);
                }

                return ServiceResult<DocumentSettingsDto>.Ok(result);
            }
            catch (Exception ex)
            {
                var message = _localizer?["FormBuilderDocumentSettings_GetError"] ?? "Error retrieving document settings";
                return ServiceResult<DocumentSettingsDto>.Error($"{message}: {ex.Message}");
            }
        }

        public async Task<ServiceResult<DocumentSettingsDto>> SaveDocumentSettingsAsync(SaveDocumentSettingsDto dto)
        {
            try
            {
                // Verify FormBuilder exists
                var formBuilder = await _unitOfWork.FormBuilderRepository.SingleOrDefaultAsync(fb => fb.Id == dto.FormBuilderId);
                if (formBuilder == null)
                {
                    var message = _localizer?["FormBuilder_NotFound"] ?? "Form Builder not found";
                    return ServiceResult<DocumentSettingsDto>.BadRequest(message);
                }

                // Check if Document Code already exists for another FormBuilder
                var existingDocType = await _unitOfWork.DocumentTypeRepository.GetByCodeAsync(dto.DocumentCode);
                if (existingDocType != null && existingDocType.FormBuilderId != dto.FormBuilderId)
                {
                    var message = _localizer?["DocumentType_CodeExists"] ?? "Document code already exists for another form";
                    return ServiceResult<DocumentSettingsDto>.BadRequest(message);
                }

                // Get or create Document Type
                // Load with no tracking to avoid navigation property update conflicts
                var dbContextForQuery = _unitOfWork.AppDbContext;
                var documentType = await dbContextForQuery.Set<DOCUMENT_TYPES>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(dt => dt.FormBuilderId == dto.FormBuilderId);

                if (documentType == null)
                {
                    // Create new Document Type
                    documentType = new DOCUMENT_TYPES
                    {
                        Name = dto.DocumentName,
                        Code = dto.DocumentCode,
                        FormBuilderId = dto.FormBuilderId,
                        MenuCaption = dto.MenuCaption,
                        MenuOrder = dto.MenuOrder,
                        ParentMenuId = dto.ParentMenuId,
                        IsActive = dto.IsActive,
                        CreatedDate = DateTime.UtcNow
                    };

                    _unitOfWork.DocumentTypeRepository.Add(documentType);
                }
                else
                {
                    // Check if ParentMenuId is being changed (removed or changed)
                    bool parentMenuIdChanged = dto.ParentMenuId.HasValue 
                        ? (documentType.ParentMenuId != dto.ParentMenuId.Value)
                        : (documentType.ParentMenuId.HasValue);

                    // If removing parent relationship (setting to null) and this entity has children
                    if (parentMenuIdChanged && (!dto.ParentMenuId.HasValue) && documentType.ParentMenuId.HasValue)
                    {
                        // Use raw SQL to update children directly - this bypasses EF tracking issues
                        // and works even if the constraint is still RESTRICT
                        var dbContext = _unitOfWork.AppDbContext;
                        await dbContext.Database.ExecuteSqlRawAsync(
                            "UPDATE DOCUMENT_TYPES SET ParentMenuId = NULL, UpdatedDate = GETUTCDATE() WHERE ParentMenuId = {0}",
                            documentType.Id);
                    }

                    // Use raw SQL to update the entity directly - this bypasses EF tracking issues
                    // and prevents conflicts with Foreign Key constraints
                    var dbContextForUpdate = _unitOfWork.AppDbContext;
                    var sqlParams = new List<object>();
                    var updateFields = new List<string>();
                    int paramIndex = 0;

                    updateFields.Add($"Name = {{{paramIndex}}}");
                    sqlParams.Add(dto.DocumentName);
                    paramIndex++;

                    updateFields.Add($"Code = {{{paramIndex}}}");
                    sqlParams.Add(dto.DocumentCode);
                    paramIndex++;

                    if (dto.MenuCaption != null)
                    {
                        updateFields.Add($"MenuCaption = {{{paramIndex}}}");
                        sqlParams.Add(dto.MenuCaption);
                        paramIndex++;
                    }

                    updateFields.Add($"MenuOrder = {{{paramIndex}}}");
                    sqlParams.Add(dto.MenuOrder);
                    paramIndex++;

                    // Handle ParentMenuId: use NULL in SQL if value is null
                    if (dto.ParentMenuId.HasValue)
                    {
                        updateFields.Add($"ParentMenuId = {{{paramIndex}}}");
                        sqlParams.Add(dto.ParentMenuId.Value);
                        paramIndex++;
                    }
                    else
                    {
                        updateFields.Add("ParentMenuId = NULL");
                    }

                    updateFields.Add($"IsActive = {{{paramIndex}}}");
                    sqlParams.Add(dto.IsActive);
                    paramIndex++;

                    updateFields.Add("UpdatedDate = GETUTCDATE()");

                    // Add id as the last parameter for WHERE clause
                    sqlParams.Add(documentType.Id);
                    var sql = $"UPDATE DOCUMENT_TYPES SET {string.Join(", ", updateFields)} WHERE Id = {{{paramIndex}}}";
                    await dbContextForUpdate.Database.ExecuteSqlRawAsync(sql, sqlParams.ToArray());

                    // Reload the entity to ensure we have the latest state (with no tracking to avoid conflicts)
                    documentType = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(documentType.Id);
                    
                    // Detach the entity from EF tracking to prevent navigation property updates
                    var dbContextForDetach = _unitOfWork.AppDbContext;
                    dbContextForDetach.Entry(documentType).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
                }

                // No need to call CompleteAsyn() here since we used raw SQL for DocumentType update
                // CompleteAsyn() will be called after DocumentSeries updates

                // Handle Document Series
                if (dto.DocumentSeries != null && dto.DocumentSeries.Any())
                {
                    foreach (var seriesDto in dto.DocumentSeries)
                    {
                        if (seriesDto.Id.HasValue)
                        {
                            // Update existing series
                            var existingSeries = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(seriesDto.Id.Value);
                            if (existingSeries != null && existingSeries.DocumentTypeId == documentType.Id)
                            {
                                // Check if series code already exists for another series
                                var codeExists = await _unitOfWork.DocumentSeriesRepository.SeriesCodeExistsAsync(
                                    seriesDto.SeriesCode, seriesDto.Id);
                                if (codeExists)
                                {
                                    var message = _localizer?["DocumentSeries_CodeExists"] ?? "Series code already exists";
                                    return ServiceResult<DocumentSettingsDto>.BadRequest(message);
                                }

                                existingSeries.ProjectId = seriesDto.ProjectId;
                                existingSeries.SeriesCode = seriesDto.SeriesCode;
                                existingSeries.NextNumber = seriesDto.NextNumber;
                                existingSeries.IsDefault = seriesDto.IsDefault;
                                existingSeries.IsActive = seriesDto.IsActive;
                                existingSeries.UpdatedDate = DateTime.UtcNow;

                                // If setting as default, remove default from other series with same document type and project
                                if (seriesDto.IsDefault)
                                {
                                    await RemoveDefaultFromOtherSeriesAsync(documentType.Id, seriesDto.ProjectId, existingSeries.Id);
                                }

                                _unitOfWork.DocumentSeriesRepository.Update(existingSeries);
                            }
                        }
                        else
                        {
                            // Create new series
                            // Check if series code already exists
                            var codeExists = await _unitOfWork.DocumentSeriesRepository.SeriesCodeExistsAsync(seriesDto.SeriesCode);
                            if (codeExists)
                            {
                                var message = _localizer?["DocumentSeries_CodeExists"] ?? "Series code already exists";
                                return ServiceResult<DocumentSettingsDto>.BadRequest(message);
                            }

                            var newSeries = new DOCUMENT_SERIES
                            {
                                DocumentTypeId = documentType.Id,
                                ProjectId = seriesDto.ProjectId,
                                SeriesCode = seriesDto.SeriesCode,
                                NextNumber = seriesDto.NextNumber,
                                IsDefault = seriesDto.IsDefault,
                                IsActive = seriesDto.IsActive,
                                CreatedDate = DateTime.UtcNow
                            };

                            // If setting as default, remove default from other series with same document type and project
                            if (seriesDto.IsDefault)
                            {
                                await RemoveDefaultFromOtherSeriesAsync(documentType.Id, seriesDto.ProjectId, null);
                            }

                            _unitOfWork.DocumentSeriesRepository.Add(newSeries);
                        }
                    }
                }

                await _unitOfWork.CompleteAsyn();

                // Return updated settings
                return await GetDocumentSettingsAsync(dto.FormBuilderId);
            }
            catch (Exception ex)
            {
                var message = _localizer?["FormBuilderDocumentSettings_SaveError"] ?? "Error saving document settings";
                return ServiceResult<DocumentSettingsDto>.Error($"{message}: {ex.Message}");
            }
        }

        public async Task<ServiceResult<bool>> DeleteDocumentSettingsAsync(int formBuilderId)
        {
            try
            {
                // Get Document Type for this FormBuilder
                var documentTypes = await _unitOfWork.DocumentTypeRepository.GetByFormBuilderIdAsync(formBuilderId);
                var documentType = documentTypes.FirstOrDefault();

                if (documentType == null)
                {
                    // Nothing to delete
                    return ServiceResult<bool>.Ok(true);
                }

                // Get all Document Series for this Document Type
                var seriesList = await _unitOfWork.DocumentSeriesRepository.GetByDocumentTypeIdAsync(documentType.Id);

                // Delete all series
                foreach (var series in seriesList)
                {
                    _unitOfWork.DocumentSeriesRepository.Delete(series);
                }

                // Delete Document Type
                _unitOfWork.DocumentTypeRepository.Delete(documentType);

                await _unitOfWork.CompleteAsyn();

                return ServiceResult<bool>.Ok(true);
            }
            catch (Exception ex)
            {
                var message = _localizer?["FormBuilderDocumentSettings_DeleteError"] ?? "Error deleting document settings";
                return ServiceResult<bool>.Error($"{message}: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove default flag from other series with the same document type and project
        /// </summary>
        private async Task RemoveDefaultFromOtherSeriesAsync(int documentTypeId, int projectId, int? excludeSeriesId)
        {
            var allSeries = await _unitOfWork.DocumentSeriesRepository.GetByDocumentTypeIdAsync(documentTypeId);
            var otherSeries = allSeries.Where(s => s.ProjectId == projectId && 
                                                   s.IsDefault && 
                                                   (!excludeSeriesId.HasValue || s.Id != excludeSeriesId.Value))
                                      .ToList();

            foreach (var series in otherSeries)
            {
                series.IsDefault = false;
                series.UpdatedDate = DateTime.UtcNow;
                _unitOfWork.DocumentSeriesRepository.Update(series);
            }
        }
    }
}

