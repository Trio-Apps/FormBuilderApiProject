using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Application.DTOs.Formula;
using FormBuilder.Domain.Interfaces.Services;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Domian.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FormBuilder.Services
{
    public class FormulaVariableService : IFormulaVariableService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormulaVariableService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<ApiResponse> GetAllAsync()
        {
            try
            {
                var items = await _unitOfWork.FormulaVariablesRepository.GetAllAsync();
                var dtos = items.Select(ToDto).ToList();
                return new ApiResponse(200, "All formula variables retrieved", dtos);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _unitOfWork.FormulaVariablesRepository.GetByIdAsync(id);
                if (entity == null)
                    return new ApiResponse(404, "Variable not found");

                return new ApiResponse(200, "Variable retrieved", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> CreateAsync(FormulaVariableCreateDto dto)
        {
            try
            {
                var entity = new FORMULA_VARIABLES
                {
                    FormulaId = dto.FormulaId,
                    VariableName = dto.VariableName,
                    SourceFieldId = dto.SourceFieldId
                };

                _unitOfWork.FormulaVariablesRepository.Add(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Variable created", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> UpdateAsync(int id, FormulaVariableUpdateDto dto)
        {
            try
            {
                var entity = (await _unitOfWork.FormulaVariablesRepository.GetAllAsync())
                    .FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return new ApiResponse(404, "Variable not found");

                if (!string.IsNullOrEmpty(dto.VariableName))
                    entity.VariableName = dto.VariableName;
                if (dto.FormulaId.HasValue)
                    entity.FormulaId = dto.FormulaId.Value;
                if (dto.SourceFieldId.HasValue)
                    entity.SourceFieldId = dto.SourceFieldId.Value;

                _unitOfWork.FormulaVariablesRepository.Update(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Variable updated", ToDto(entity));
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> DeleteAsync(int id)
        {
            try
            {
                var entity = (await _unitOfWork.FormulaVariablesRepository.GetAllAsync())
                    .FirstOrDefault(x => x.Id == id);
                if (entity == null)
                    return new ApiResponse(404, "Variable not found");

                _unitOfWork.FormulaVariablesRepository.Delete(entity);
                await _unitOfWork.CompleteAsyn();

                return new ApiResponse(200, "Variable deleted");
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse> ExistsAsync(int id)
        {
            try
            {
                var exists = await _unitOfWork.FormulaVariablesRepository.AnyAsync(x => x.Id == id);
                return new ApiResponse(200, "Existence checked", exists);
            }
            catch (Exception ex)
            {
                return new ApiResponse(500, $"Error: {ex.Message}");
            }
        }

        private FormulaVariableDto ToDto(FORMULA_VARIABLES e)
        {
            return new FormulaVariableDto
            {
                Id = e.Id,
                VariableName = e.VariableName,
                FormulaId = e.FormulaId,
                Formulaname = e.FORMULAS?.Name,
                SourceFieldId = e.SourceFieldId,
                SourceFieldName = e.FORM_FIELDS?.FieldName
            };
        }
    }
}
