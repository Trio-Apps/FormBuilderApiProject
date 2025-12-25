using formBuilder.Domian.Interfaces;
using FormBuilder.Core.DTOS.FormBuilder;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Services.Services.Base;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.API.Models;
using FormBuilder.API.DTOs;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormSubmissionsService : BaseService<FORM_SUBMISSIONS, FormSubmissionDto, CreateFormSubmissionDto, UpdateFormSubmissionDto>, IFormSubmissionsService
    {
        private readonly IunitOfwork _unitOfWork;
        private readonly IFormSubmissionGridRowService _formSubmissionGridRowService;
        private readonly IFormSubmissionValuesService _formSubmissionValuesService;

        public FormSubmissionsService(
            IunitOfwork unitOfWork, 
            IMapper mapper,
            IFormSubmissionGridRowService formSubmissionGridRowService,
            IFormSubmissionValuesService formSubmissionValuesService) : base(unitOfWork, mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _formSubmissionGridRowService = formSubmissionGridRowService ?? throw new ArgumentNullException(nameof(formSubmissionGridRowService));
            _formSubmissionValuesService = formSubmissionValuesService ?? throw new ArgumentNullException(nameof(formSubmissionValuesService));
        }

        protected override IBaseRepository<FORM_SUBMISSIONS> Repository => _unitOfWork.FormSubmissionsRepository;

        public async Task<ApiResponse> GetAllAsync()
        {
            var submissions = await _unitOfWork.FormSubmissionsRepository.GetSubmissionsWithDetailsAsync();
            var submissionDtos = _mapper.Map<IEnumerable<FormSubmissionDto>>(submissions);
            return new ApiResponse(200, "All form submissions retrieved successfully", submissionDtos);
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            var submission = await _unitOfWork.FormSubmissionsRepository.GetByIdWithDetailsAsync(id);
            if (submission == null)
                return new ApiResponse(404, "Form submission not found");

            var submissionDto = _mapper.Map<FormSubmissionDetailDto>(submission);
            return new ApiResponse(200, "Form submission retrieved successfully", submissionDto);
        }

        public async Task<ApiResponse> GetByIdWithDetailsAsync(int id)
        {
            var submission = await _unitOfWork.FormSubmissionsRepository.GetByIdWithDetailsAsync(id);
            if (submission == null)
                return new ApiResponse(404, "Form submission not found");

            var submissionDto = _mapper.Map<FormSubmissionDetailDto>(submission);
            
            // Map nested collections if needed
            if (submission.FORM_SUBMISSION_VALUES != null)
            {
                submissionDto.FieldValues = _mapper.Map<List<FormSubmissionValueDto>>(submission.FORM_SUBMISSION_VALUES);
            }
            if (submission.FORM_SUBMISSION_ATTACHMENTS != null)
            {
                submissionDto.Attachments = _mapper.Map<List<FormSubmissionAttachmentDto>>(submission.FORM_SUBMISSION_ATTACHMENTS);
            }
            if (submission.FORM_SUBMISSION_GRID_ROWS != null)
            {
                // Map grid rows with cells
                submissionDto.GridData = submission.FORM_SUBMISSION_GRID_ROWS
                    .Select(row => new FormSubmissionGridDto
                    {
                        Id = row.Id,
                        SubmissionId = row.SubmissionId,
                        GridId = row.GridId,
                        GridName = row.FORM_GRIDS?.GridName ?? string.Empty,
                        GridCode = row.FORM_GRIDS?.GridCode ?? string.Empty,
                        RowIndex = row.RowIndex,
                        Cells = row.FORM_SUBMISSION_GRID_CELLS != null
                            ? row.FORM_SUBMISSION_GRID_CELLS.Select(c => new FormBuilder.Core.DTOS.FormBuilder.FormSubmissionGridCellDto
                            {
                                Id = c.Id,
                                RowId = c.RowId,
                                ColumnId = c.ColumnId,
                                ColumnCode = c.FORM_GRID_COLUMNS?.ColumnCode ?? string.Empty,
                                ColumnName = c.FORM_GRID_COLUMNS?.ColumnName ?? string.Empty,
                                ValueString = c.ValueString,
                                ValueNumber = c.ValueNumber,
                                ValueDate = c.ValueDate,
                                ValueBool = c.ValueBool,
                                ValueJson = c.ValueJson
                            }).ToList()
                            : new List<FormBuilder.Core.DTOS.FormBuilder.FormSubmissionGridCellDto>()
                    }).ToList();
            }

            return new ApiResponse(200, "Form submission with details retrieved successfully", submissionDto);
        }

        public async Task<ApiResponse> GetByDocumentNumberAsync(string documentNumber)
        {
            var submission = await _unitOfWork.FormSubmissionsRepository.GetByDocumentNumberAsync(documentNumber);
            if (submission == null)
                return new ApiResponse(404, "Form submission not found");

            var submissionDto = _mapper.Map<FormSubmissionDto>(submission);
            return new ApiResponse(200, "Form submission retrieved successfully", submissionDto);
        }

        public async Task<ApiResponse> GetByFormBuilderIdAsync(int formBuilderId)
        {
            var submissions = await _unitOfWork.FormSubmissionsRepository.GetByFormBuilderIdAsync(formBuilderId);
            var submissionDtos = _mapper.Map<IEnumerable<FormSubmissionDto>>(submissions);
            return new ApiResponse(200, "Form submissions retrieved successfully", submissionDtos);
        }

        public async Task<ApiResponse> GetByDocumentTypeIdAsync(int documentTypeId)
        {
            var submissions = await _unitOfWork.FormSubmissionsRepository.GetByDocumentTypeIdAsync(documentTypeId);
            var submissionDtos = _mapper.Map<IEnumerable<FormSubmissionDto>>(submissions);
            return new ApiResponse(200, "Form submissions retrieved successfully", submissionDtos);
        }

        public async Task<ApiResponse> GetByUserIdAsync(string userId)
        {
            var submissions = await _unitOfWork.FormSubmissionsRepository.GetByUserIdAsync(userId);
            var submissionDtos = _mapper.Map<IEnumerable<FormSubmissionDto>>(submissions);
            return new ApiResponse(200, "User form submissions retrieved successfully", submissionDtos);
        }

        public async Task<ApiResponse> GetByStatusAsync(string status)
        {
            var submissions = await _unitOfWork.FormSubmissionsRepository.GetByStatusAsync(status);
            var submissionDtos = _mapper.Map<IEnumerable<FormSubmissionDto>>(submissions);
            return new ApiResponse(200, "Form submissions by status retrieved successfully", submissionDtos);
        }

        public async Task<ApiResponse> CreateAsync(CreateFormSubmissionDto createDto)
        {
            if (createDto == null)
                return new ApiResponse(400, "DTO is required");

            // Generate document number
            var series = await _unitOfWork.DocumentSeriesRepository.GetByIdAsync(createDto.SeriesId);
            if (series == null)
                return new ApiResponse(404, "Document series not found");

            var nextNumber = await _unitOfWork.DocumentSeriesRepository.GetNextNumberAsync(createDto.SeriesId);
            var documentNumber = $"{series.SeriesCode}-{nextNumber:D6}";

            // Get next version
            var version = await _unitOfWork.FormSubmissionsRepository.GetNextVersionAsync(createDto.FormBuilderId);

            var entity = _mapper.Map<FORM_SUBMISSIONS>(createDto);
            entity.DocumentNumber = documentNumber;
            entity.Version = version;
            entity.SubmittedDate = DateTime.UtcNow;
            entity.CreatedDate = DateTime.UtcNow;
            entity.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.FormSubmissionsRepository.Add(entity);
            await _unitOfWork.CompleteAsyn();

            var createdEntity = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(entity.Id);
            var submissionDto = _mapper.Map<FormSubmissionDto>(createdEntity);
            return new ApiResponse(200, "Form submission created successfully", submissionDto);
        }

        /// <summary>
        /// Create a new draft submission automatically determining Document Type and Series from FormBuilderId
        /// This method implements the runtime flow as specified in the requirements:
        /// 1. Loads the form (FormBuilderId)
        /// 2. Loads the linked Document Type from DOCUMENT_TYPES
        /// 3. Selects the correct Document Series from DOCUMENT_SERIES (default series for the project)
        /// 4. Generates Document Number (Prefix + NextNumber)
        /// 5. Creates a record in FORM_SUBMISSIONS with Status = Draft
        /// </summary>
        public async Task<ApiResponse> CreateDraftAsync(int formBuilderId, int projectId, string submittedByUserId)
        {
            // 1. Verify FormBuilder exists
            var formBuilder = await _unitOfWork.FormBuilderRepository.SingleOrDefaultAsync(fb => fb.Id == formBuilderId);
            if (formBuilder == null)
                return new ApiResponse(404, "Form Builder not found");

            // 2. Load the linked Document Type from DOCUMENT_TYPES
            var documentTypes = await _unitOfWork.DocumentTypeRepository.GetByFormBuilderIdAsync(formBuilderId);
            var documentType = documentTypes.FirstOrDefault();
            if (documentType == null)
                return new ApiResponse(404, "No Document Type configured for this form. Please configure Document Settings in Form Builder.");

            if (!documentType.IsActive)
                return new ApiResponse(400, "Document Type is not active");

            // 3. Select the correct Document Series from DOCUMENT_SERIES (default series for the project)
            var series = await _unitOfWork.DocumentSeriesRepository.GetDefaultSeriesAsync(documentType.Id, projectId);
            if (series == null)
            {
                // If no default series, try to get any active series for this document type and project
                var allSeries = await _unitOfWork.DocumentSeriesRepository.GetByDocumentTypeIdAsync(documentType.Id);
                series = allSeries.FirstOrDefault(s => s.ProjectId == projectId && s.IsActive);
                
                if (series == null)
                    return new ApiResponse(404, $"No active Document Series found for Document Type '{documentType.Name}' and Project ID {projectId}. Please configure Document Series in Form Builder Document Settings.");
            }

            if (!series.IsActive)
                return new ApiResponse(400, "Document Series is not active");

            // 4. Generate Document Number (Prefix + NextNumber)
            // Use database transaction/locking when incrementing NextNumber
            var nextNumber = await _unitOfWork.DocumentSeriesRepository.GetNextNumberAsync(series.Id);
            var documentNumber = $"{series.SeriesCode}-{nextNumber:D6}";

            // 5. Get next version
            var version = await _unitOfWork.FormSubmissionsRepository.GetNextVersionAsync(formBuilderId);

            // 6. Create a record in FORM_SUBMISSIONS with Status = Draft
            var entity = new FORM_SUBMISSIONS
            {
                FormBuilderId = formBuilderId,
                Version = version,
                DocumentTypeId = documentType.Id,
                SeriesId = series.Id,
                DocumentNumber = documentNumber,
                SubmittedByUserId = submittedByUserId,
                SubmittedDate = DateTime.UtcNow,
                Status = "Draft",
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _unitOfWork.FormSubmissionsRepository.Add(entity);
            await _unitOfWork.CompleteAsyn();

            var createdEntity = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(entity.Id);
            var submissionDto = _mapper.Map<FormSubmissionDto>(createdEntity);
            return new ApiResponse(200, "Draft form submission created successfully", submissionDto);
        }

        public async Task<ApiResponse> UpdateAsync(int id, UpdateFormSubmissionDto updateDto)
        {
            if (updateDto == null)
                return new ApiResponse(400, "DTO is required");

            var entity = await _unitOfWork.FormSubmissionsRepository.SingleOrDefaultAsync(s => s.Id == id, asNoTracking: false);
            if (entity == null)
                return new ApiResponse(404, "Form submission not found");

            // Check document number uniqueness if changed
            if (!string.IsNullOrEmpty(updateDto.DocumentNumber) && updateDto.DocumentNumber != entity.DocumentNumber)
            {
                var documentNumberExists = await _unitOfWork.FormSubmissionsRepository.DocumentNumberExistsAsync(updateDto.DocumentNumber);
                if (documentNumberExists)
                    return new ApiResponse(400, "Document number already exists");
            }

            _mapper.Map(updateDto, entity);
            entity.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.FormSubmissionsRepository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            var updatedEntity = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(id);
            var submissionDto = _mapper.Map<FormSubmissionDto>(updatedEntity);
            return new ApiResponse(200, "Form submission updated successfully", submissionDto);
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            var result = await base.DeleteAsync(id);
            return ConvertToApiResponse(result);
        }

        public async Task<ApiResponse> SubmitAsync(SubmitFormDto submitDto)
        {
            var entity = await _unitOfWork.FormSubmissionsRepository.SingleOrDefaultAsync(s => s.Id == submitDto.SubmissionId, asNoTracking: false);
            if (entity == null)
                return new ApiResponse(404, "Form submission not found");

            if (entity.Status == "Submitted")
                return new ApiResponse(400, "Form submission is already submitted");

            entity.Status = "Submitted";
            entity.SubmittedDate = DateTime.UtcNow;
            entity.SubmittedByUserId = submitDto.SubmittedByUserId;
            entity.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.FormSubmissionsRepository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            var submissionDto = _mapper.Map<FormSubmissionDto>(entity);
            return new ApiResponse(200, "Form submission submitted successfully", submissionDto);
        }

        public async Task<ApiResponse> UpdateStatusAsync(int id, string status)
        {
            var entity = await _unitOfWork.FormSubmissionsRepository.SingleOrDefaultAsync(s => s.Id == id, asNoTracking: false);
            if (entity == null)
                return new ApiResponse(404, "Form submission not found");

            entity.Status = status;
            entity.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.FormSubmissionsRepository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            var updatedEntity = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(id);
            var submissionDto = _mapper.Map<FormSubmissionDto>(updatedEntity);
            return new ApiResponse(200, $"Form submission status updated to {status} successfully", submissionDto);
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            var exists = await _unitOfWork.FormSubmissionsRepository.AnyAsync(s => s.Id == id);
            return new ApiResponse(200, "Form submission existence checked successfully", exists);
        }

        public async Task<ApiResponse> SaveFormSubmissionDataAsync(SaveFormSubmissionDataDto saveDto)
        {
            if (saveDto == null)
                return new ApiResponse(400, "DTO is required");

            // التحقق من Submission
            var submission = await _unitOfWork.FormSubmissionsRepository.GetByIdAsync(saveDto.SubmissionId);
            if (submission == null)
                return new ApiResponse(404, "Submission not found");

            // حفظ Field Values
            if (saveDto.FieldValues != null && saveDto.FieldValues.Any())
            {
                // Convert BulkSaveFieldValuesDto to BulkFormSubmissionValuesDto
                var bulkFieldValuesDto = new BulkFormSubmissionValuesDto
                {
                    SubmissionId = saveDto.SubmissionId,
                    Values = saveDto.FieldValues.Select(fv => new CreateFormSubmissionValueDto
                    {
                        SubmissionId = saveDto.SubmissionId,
                        FieldId = fv.FieldId,
                        FieldCode = fv.FieldCode ?? "",
                        ValueString = fv.ValueString,
                        ValueNumber = fv.ValueNumber,
                        ValueDate = fv.ValueDate,
                        ValueBool = fv.ValueBool,
                        ValueJson = fv.ValueJson
                    }).ToList()
                };
                var result = await _formSubmissionValuesService.CreateBulkAsync(bulkFieldValuesDto);
                if (result.StatusCode != 200)
                    return result;
            }

            // حفظ Attachments
            if (saveDto.Attachments != null && saveDto.Attachments.Any())
            {
                // Attachment saving logic can be added here if needed
                // For now, attachments are handled separately
            }

            // حفظ Grid Data
            if (saveDto.GridData != null && saveDto.GridData.Any())
            {
                // تجميع Grid data حسب GridId
                var gridDataGroups = saveDto.GridData.GroupBy(g => g.GridId);
                
                foreach (var group in gridDataGroups)
                {
                    var gridId = group.Key;
                    var rows = group.ToList();
                    
                    var bulkDto = new BulkSaveGridDataDto
                    {
                        SubmissionId = saveDto.SubmissionId,
                        GridId = gridId,
                        Rows = rows
                    };
                    
                    // التحقق من البيانات أولاً
                    var validationResult = await _formSubmissionGridRowService.ValidateGridDataAsync(bulkDto);
                    if (validationResult.StatusCode == 200)
                    {
                        var validationData = validationResult.Data as GridValidationResultDto;
                        if (validationData != null && !validationData.IsValid)
                        {
                            return new ApiResponse(400, "Grid validation failed", validationData);
                        }
                    }
                    
                    // حفظ البيانات
                    await _formSubmissionGridRowService.SaveBulkGridDataAsync(bulkDto);
                }
            }

            return new ApiResponse(200, "Form submission data saved successfully");
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
