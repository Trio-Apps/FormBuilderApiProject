using AutoMapper;
using FormBuilder.API.Models.DTOs;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.FromBuilder;
using formBuilder.Domian.Interfaces;
using FormBuilder.Services.Services.Base;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FormBuilder.Services
{
    public class DocumentTypeService
        : BaseService<DOCUMENT_TYPES, DocumentTypeDto, CreateDocumentTypeDto, UpdateDocumentTypeDto>,
          IDocumentTypeService
    {
        private readonly IStringLocalizer<DocumentTypeService>? _localizer;

        public DocumentTypeService(IunitOfwork unitOfWork, IMapper mapper, IStringLocalizer<DocumentTypeService>? localizer = null)
            : base(unitOfWork, mapper, null)
        {
            _localizer = localizer;
        }

        protected override IBaseRepository<DOCUMENT_TYPES> Repository => _unitOfWork.DocumentTypeRepository;

        public async Task<ServiceResult<IEnumerable<DocumentTypeDto>>> GetAllAsync(Expression<Func<DOCUMENT_TYPES, bool>>? filter = null)
        {
            var list = await _unitOfWork.DocumentTypeRepository.GetAllAsync(filter, dt => dt.FORM_BUILDER, dt => dt.ParentMenu);
            return ServiceResult<IEnumerable<DocumentTypeDto>>.Ok(_mapper.Map<IEnumerable<DocumentTypeDto>>(list));
        }

        public async Task<ServiceResult<PagedResult<DocumentTypeDto>>> GetPagedAsync(int page = 1, int pageSize = 20, Expression<Func<DOCUMENT_TYPES, bool>>? filter = null)
        {
            // leverage base pagination on GetAll with includes
            if (page <= 0) page = 1;
            if (pageSize <= 0) pageSize = 20;

            var query = Repository.GetAll();
            if (filter != null) query = query.Where(filter);
            query = query
                .Include(dt => dt.FORM_BUILDER)
                .Include(dt => dt.ParentMenu);

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(dt => dt.MenuOrder)
                .ThenBy(dt => dt.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var mapped = _mapper.Map<IEnumerable<DocumentTypeDto>>(items);
            return ServiceResult<PagedResult<DocumentTypeDto>>.Ok(new PagedResult<DocumentTypeDto>(mapped, total, page, pageSize));
        }

        public async Task<ServiceResult<DocumentTypeDto>> GetByIdAsync(int id, bool asNoTracking = false)
        {
            var entity = await _unitOfWork.DocumentTypeRepository.GetByIdAsync(id);
            if (entity == null) return ServiceResult<DocumentTypeDto>.NotFound();
            return ServiceResult<DocumentTypeDto>.Ok(_mapper.Map<DocumentTypeDto>(entity));
        }

        public async Task<ServiceResult<DocumentTypeDto>> GetByCodeAsync(string code, bool asNoTracking = false)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                var message = _localizer?["DocumentType_CodeRequired"] ?? "Code is required";
                return ServiceResult<DocumentTypeDto>.BadRequest(message);
            }

            var entity = await _unitOfWork.DocumentTypeRepository.GetByCodeAsync(code.Trim());
            if (entity == null) return ServiceResult<DocumentTypeDto>.NotFound();

            return ServiceResult<DocumentTypeDto>.Ok(_mapper.Map<DocumentTypeDto>(entity));
        }

        public async Task<ServiceResult<IEnumerable<DocumentTypeDto>>> GetByFormBuilderIdAsync(int formBuilderId, bool asNoTracking = false)
        {
            var list = await _unitOfWork.DocumentTypeRepository.GetByFormBuilderIdAsync(formBuilderId);
            return ServiceResult<IEnumerable<DocumentTypeDto>>.Ok(_mapper.Map<IEnumerable<DocumentTypeDto>>(list));
        }

        public async Task<ServiceResult<IEnumerable<DocumentTypeDto>>> GetActiveAsync()
        {
            var list = await _unitOfWork.DocumentTypeRepository.GetActiveAsync();
            return ServiceResult<IEnumerable<DocumentTypeDto>>.Ok(_mapper.Map<IEnumerable<DocumentTypeDto>>(list));
        }

        public async Task<ServiceResult<IEnumerable<DocumentTypeDto>>> GetByParentMenuIdAsync(int? parentMenuId, bool asNoTracking = false)
        {
            var list = await _unitOfWork.DocumentTypeRepository.GetByParentMenuIdAsync(parentMenuId);
            return ServiceResult<IEnumerable<DocumentTypeDto>>.Ok(_mapper.Map<IEnumerable<DocumentTypeDto>>(list));
        }

        public override async Task<ServiceResult<DocumentTypeDto>> CreateAsync(CreateDocumentTypeDto createDto)
        {
            return await base.CreateAsync(createDto);
        }

        public override async Task<ServiceResult<DocumentTypeDto>> UpdateAsync(int id, UpdateDocumentTypeDto updateDto)
        {
            return await base.UpdateAsync(id, updateDto);
        }

        public override async Task<ServiceResult<bool>> DeleteAsync(int id)
        {
            return await base.DeleteAsync(id);
        }

        public async Task<ServiceResult<bool>> ToggleActiveAsync(int id, bool isActive)
        {
            var entity = await Repository.SingleOrDefaultAsync(dt => dt.Id == id);
            if (entity == null) return ServiceResult<bool>.NotFound();

            entity.IsActive = isActive;
            entity.UpdatedDate = DateTime.UtcNow;
            Repository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<bool>> ExistsAsync(int id)
        {
            var exists = await Repository.AnyAsync(dt => dt.Id == id);
            return ServiceResult<bool>.Ok(exists);
        }

        public async Task<ServiceResult<bool>> CodeExistsAsync(string code, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                var message = _localizer?["DocumentType_CodeRequired"] ?? "Code is required";
                return ServiceResult<bool>.BadRequest(message);
            }

            var exists = await _unitOfWork.DocumentTypeRepository.CodeExistsAsync(code.Trim(), excludeId);
            return ServiceResult<bool>.Ok(exists);
        }

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateDocumentTypeDto dto)
        {
            if (dto == null)
            {
                var message = _localizer?["Common_PayloadRequired"] ?? "Payload is required";
                return ValidationResult.Failure(message);
            }

            var exists = await _unitOfWork.DocumentTypeRepository.CodeExistsAsync(dto.Code);
            if (exists)
            {
                var message = _localizer?["DocumentType_CodeExists", dto.Code] ?? $"Document type code '{dto.Code}' already exists.";
                return ValidationResult.Failure(message);
            }

            if (dto.FormBuilderId.HasValue)
            {
                var formBuilderExists = await _unitOfWork.FormBuilderRepository.AnyAsync(e => e.Id == dto.FormBuilderId.Value);
                if (!formBuilderExists)
                {
                    var message = _localizer?["DocumentType_InvalidFormBuilderId"] ?? "Invalid form builder ID";
                    return ValidationResult.Failure(message);
                }
            }

            if (dto.ParentMenuId.HasValue)
            {
                var parentExists = await _unitOfWork.DocumentTypeRepository.AnyAsync(e => e.Id == dto.ParentMenuId.Value);
                if (!parentExists) return ValidationResult.Failure("Invalid parent menu ID");
            }

            return ValidationResult.Success();
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateDocumentTypeDto dto, DOCUMENT_TYPES entity)
        {
            if (dto == null) return ValidationResult.Failure("Payload is required");

            if (!string.IsNullOrWhiteSpace(dto.Code) && !string.Equals(dto.Code, entity.Code, StringComparison.OrdinalIgnoreCase))
            {
                var exists = await _unitOfWork.DocumentTypeRepository.CodeExistsAsync(dto.Code, id);
                if (exists) return ValidationResult.Failure($"Document type code '{dto.Code}' already exists.");
            }

            if (dto.FormBuilderId.HasValue)
            {
                var formBuilderExists = await _unitOfWork.FormBuilderRepository.AnyAsync(e => e.Id == dto.FormBuilderId.Value);
                if (!formBuilderExists) return ValidationResult.Failure("Invalid form builder ID");
            }

            if (dto.ParentMenuId.HasValue)
            {
                var parentExists = await _unitOfWork.DocumentTypeRepository.AnyAsync(e => e.Id == dto.ParentMenuId.Value);
                if (!parentExists) return ValidationResult.Failure("Invalid parent menu ID");
            }

            return ValidationResult.Success();
        }
    }
}