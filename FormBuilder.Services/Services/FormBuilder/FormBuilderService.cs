using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using FormBuilder.Core.IServices.FormBuilder.FormBuilder.Services.Services;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FormBuilderService : IFormBuilderService
    {
        // Dependency on the Unit of Work
        private readonly IunitOfwork _unitOfWork;

        public FormBuilderService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        // --- CRUD Operations ---

        public async Task<FORM_BUILDER> CreateFormAsync(FORM_BUILDER form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            // Validate form code uniqueness using the specific repository (FormBuilderRepository)
            // Note: The original code had a slight error here, using SingleOrDefaultAsync which returns T?, not bool.
            // We use the dedicated IFormBuilderRepository method exposed via UoW.
            var formCodeExists = await _unitOfWork.FormBuilderRepository.IsFormCodeExistsAsync(form.FormCode);
            if (formCodeExists)
                throw new InvalidOperationException($"Form code '{form.FormCode}' already exists.");

            // Set creation details
            form.CreatedDate = DateTime.UtcNow;
            form.IsActive = true;

            // Use generic repository to add the form
            var formRepo = _unitOfWork.Repositary<FORM_BUILDER>(); // Using Repositary<T> spelling from UoW
            formRepo.Add(form);

            await _unitOfWork.CompleteAsyn(); // Using CompleteAsyn spelling from UoW

            return form;
        }

        public async Task<FORM_BUILDER> UpdateFormAsync(FORM_BUILDER form)
        {
            if (form == null)
                throw new ArgumentNullException(nameof(form));

            // Validate form code uniqueness, excluding the current form's Id
            var exists = await _unitOfWork.FormBuilderRepository.IsFormCodeExistsAsync(form.FormCode, form.Id);
            if (exists)
                throw new InvalidOperationException($"Form code '{form.FormCode}' already exists.");

            // Set modification details
            form.UpdatedDate = DateTime.UtcNow;

            // Use generic repository to update the form
            var formRepo = _unitOfWork.Repositary<FORM_BUILDER>();
            var updatedForm = formRepo.Update(form);

            await _unitOfWork.CompleteAsyn();

            return updatedForm;
        }

        public async Task<bool> DeleteFormAsync(int id)
        {
            var formRepo = _unitOfWork.Repositary<FORM_BUILDER>();

            // First, retrieve the entity to be deleted
            var form = await formRepo.SingleOrDefaultAsync(f => f.Id == id);

            if (form == null)
                return false;

            // Stage the delete operation
            formRepo.Delete(form);

            // Commit the transaction
            await _unitOfWork.CompleteAsyn();

            return true;
        }

        // --- Retrieval Operations ---

        public async Task<FORM_BUILDER?> GetFormByIdAsync(int id, bool asNoTracking = false)
        {
            var formRepo = _unitOfWork.Repositary<FORM_BUILDER>();
            return await formRepo.SingleOrDefaultAsync(
                f => f.Id == id,
                asNoTracking: asNoTracking);
        }

        public async Task<FORM_BUILDER?> GetFormByCodeAsync(string formCode, bool asNoTracking = false)
        {
            if (string.IsNullOrWhiteSpace(formCode))
                return null;

            var formRepo = _unitOfWork.Repositary<FORM_BUILDER>();
            return await formRepo.SingleOrDefaultAsync(
                f => f.FormCode == formCode.Trim(),
                asNoTracking: asNoTracking);
        }

        public async Task<IEnumerable<FORM_BUILDER>> GetAllFormsAsync(Expression<Func<FORM_BUILDER, bool>>? filter = null)
        {
            var formRepo = _unitOfWork.Repositary<FORM_BUILDER>();
            // The GetAllAsync method handles the filter and returns an ICollection (List)
            return await formRepo.GetAllAsync(filter);
        }

        // --- Utility and Validation ---

        public async Task<bool> IsFormCodeExistsAsync(string formCode, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(formCode))
                return false;

            // Delegates the complex check to the specific repository
            return await _unitOfWork.FormBuilderRepository.IsFormCodeExistsAsync(formCode.Trim(), excludeId);
        }

        public async Task<int> GetFormsCountAsync(Expression<Func<FORM_BUILDER, bool>>? filter = null)
        {
            var formRepo = _unitOfWork.Repositary<FORM_BUILDER>();
            return await formRepo.CountAsync(filter);
        }

        public async Task<bool> AnyFormsAsync(Expression<Func<FORM_BUILDER, bool>>? filter = null)
        {
            var formRepo = _unitOfWork.Repositary<FORM_BUILDER>();
            return await formRepo.AnyAsync(filter);
        }
    }
}