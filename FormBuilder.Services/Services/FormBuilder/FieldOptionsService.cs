using AutoMapper;
using formBuilder.Domian.Interfaces;
using FormBuilder.Application.DTOS;
using FormBuilder.Core.DTOS.Common;
using FormBuilder.Core.IServices.FormBuilder;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.API.Models;
using FormBuilder.Services.Services.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using CreateFieldOptionDto = FormBuilder.API.Models.CreateFieldOptionDto;
using UpdateFieldOptionDto = FormBuilder.API.Models.UpdateFieldOptionDto;

namespace FormBuilder.Services.Services
{
    public class FieldOptionsService : BaseService<FIELD_OPTIONS, FieldOptionDto, CreateFieldOptionDto, UpdateFieldOptionDto>, IFieldOptionsService
    {
        public FieldOptionsService(IunitOfwork unitOfWork, IMapper mapper)
            : base(unitOfWork, mapper)
        {
        }

        protected override IBaseRepository<FIELD_OPTIONS> Repository => _unitOfWork.FieldOptionsRepository;

        // ================================
        // CUSTOM OPERATIONS
        // ================================

        public async Task<ServiceResult<IEnumerable<FieldOptionDto>>> GetByFieldIdAsync(int fieldId)
        {
            var options = await _unitOfWork.FieldOptionsRepository.GetByFieldIdAsync(fieldId);
            var dtos = _mapper.Map<IEnumerable<FieldOptionDto>>(options);
            return ServiceResult<IEnumerable<FieldOptionDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<IEnumerable<FieldOptionDto>>> GetActiveByFieldIdAsync(int fieldId)
        {
            var options = await _unitOfWork.FieldOptionsRepository.GetActiveByFieldIdAsync(fieldId);
            var dtos = _mapper.Map<IEnumerable<FieldOptionDto>>(options);
            return ServiceResult<IEnumerable<FieldOptionDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<IEnumerable<FieldOptionDto>>> CreateBulkAsync(List<CreateFieldOptionDto> createDtos)
        {
            if (createDtos == null || !createDtos.Any())
                return ServiceResult<IEnumerable<FieldOptionDto>>.BadRequest("No field options provided");

            // Validate all field IDs exist
            var fieldIds = createDtos.Select(d => d.FieldId).Distinct().ToList();
            foreach (var fieldId in fieldIds)
            {
                var fieldExists = await _unitOfWork.FormFieldRepository.AnyAsync(f => f.Id == fieldId);
                if (!fieldExists)
                    return ServiceResult<IEnumerable<FieldOptionDto>>.BadRequest($"Invalid field ID: {fieldId}");
            }

            var entities = _mapper.Map<List<FIELD_OPTIONS>>(createDtos);
            foreach (var entity in entities)
            {
                entity.CreatedDate = DateTime.UtcNow;
                entity.IsActive = true;
            }

            Repository.AddRange(entities);
            await _unitOfWork.CompleteAsyn();

            var dtos = _mapper.Map<IEnumerable<FieldOptionDto>>(entities);
            return ServiceResult<IEnumerable<FieldOptionDto>>.Ok(dtos);
        }

        public async Task<ServiceResult<bool>> SoftDeleteAsync(int id)
        {
            var entity = await Repository.SingleOrDefaultAsync(e => e.Id == id);
            if (entity == null)
                return ServiceResult<bool>.NotFound();

            entity.IsActive = false;
            entity.UpdatedDate = DateTime.UtcNow;
            Repository.Update(entity);
            await _unitOfWork.CompleteAsyn();

            return ServiceResult<bool>.Ok(true);
        }

        public async Task<ServiceResult<FieldOptionDto>> GetDefaultOptionAsync(int fieldId)
        {
            var defaultOption = await _unitOfWork.FieldOptionsRepository.GetDefaultOptionAsync(fieldId);
            if (defaultOption == null)
                return ServiceResult<FieldOptionDto>.NotFound("No default option found for this field");

            var dto = _mapper.Map<FieldOptionDto>(defaultOption);
            return ServiceResult<FieldOptionDto>.Ok(dto);
        }

        public async Task<ServiceResult<int>> GetOptionsCountAsync(int fieldId)
        {
            var count = await _unitOfWork.FieldOptionsRepository.GetOptionsCountAsync(fieldId);
            return ServiceResult<int>.Ok(count);
        }

        public async Task<bool> FieldHasOptionsAsync(int fieldId)
        {
            return await _unitOfWork.FieldOptionsRepository.FieldHasOptionsAsync(fieldId);
        }

        // ================================
        // VALIDATION OVERRIDES
        // ================================

        protected override async Task<ValidationResult> ValidateCreateAsync(CreateFieldOptionDto dto)
        {
            // Validate if field exists
            var fieldExists = await _unitOfWork.FormFieldRepository.AnyAsync(f => f.Id == dto.FieldId);
            if (!fieldExists)
                return ValidationResult.Failure($"Invalid field ID: {dto.FieldId}");

            return ValidationResult.Success();
        }

        protected override async Task<ValidationResult> ValidateUpdateAsync(int id, UpdateFieldOptionDto dto, FIELD_OPTIONS entity)
        {
            // Additional validation can be added here if needed
            return ValidationResult.Success();
        }
    }
}