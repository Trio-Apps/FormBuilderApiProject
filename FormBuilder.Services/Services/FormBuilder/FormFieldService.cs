using formBuilder.Domian.Interfaces;
using FormBuilder.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FormFieldService : IFormFieldService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormFieldService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<FORM_FIELDS> CreateFieldAsync(FORM_FIELDS fieldEntity)
        {
            if (fieldEntity == null)
                throw new ArgumentNullException(nameof(fieldEntity));

            // 1. التحقق من وجود التبويب الأب
            var tabExists = await _unitOfWork.Repositary<FORM_TABS>()
                .SingleOrDefaultAsync(t => t.id == fieldEntity.TabId, asNoTracking: true);

            if (tabExists == null)
                throw new InvalidOperationException($"Tab with ID '{fieldEntity.TabId}' does not exist.");

            // 2. التحقق من وجود نوع الحقل
            var fieldTypeExists = await _unitOfWork.Repositary<FIELD_TYPES>()
                .SingleOrDefaultAsync(ft => ft.id == fieldEntity.FieldTypeId, asNoTracking: true);

            if (fieldTypeExists == null)
                throw new InvalidOperationException($"Field type with ID '{fieldEntity.FieldTypeId}' does not exist.");

            // 3. التحقق من تكرار FieldCode
            if (!await IsFieldCodeUniqueAsync(fieldEntity.FieldCode))
                throw new InvalidOperationException($"Field code '{fieldEntity.FieldCode}' is already in use.");

            // 4. التحقق من تكرار FieldName في نفس التبويب
            if (!await IsFieldNameUniqueAsync(fieldEntity.FieldName, null, fieldEntity.TabId))
                throw new InvalidOperationException($"Field name '{fieldEntity.FieldName}' is already in use in this tab.");

            // 5. تعيين القيم الافتراضية
            if (fieldEntity.CreatedDate == default)
                fieldEntity.CreatedDate = DateTime.UtcNow;

            fieldEntity.IsActive = true;

            // تعيين القيم الافتراضية إذا لم يتم تحديدها
            fieldEntity.IsVisible = fieldEntity.IsVisible;
            fieldEntity.IsEditable = fieldEntity.IsEditable;
            fieldEntity.FieldOrder = fieldEntity.FieldOrder;

            // 6. إضافة الحقل وحفظ التغييرات
            _unitOfWork.Repositary<FORM_FIELDS>().Add(fieldEntity);
            await _unitOfWork.CompleteAsyn();

            return fieldEntity;
        }

        public async Task<FORM_FIELDS?> GetFieldByIdAsync(int id, bool asNoTracking = false)
        {
            return await _unitOfWork.Repositary<FORM_FIELDS>()
                .SingleOrDefaultAsync(f => f.id == id, asNoTracking: asNoTracking);
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByTabIdAsync(int tabId)
        {
            return await _unitOfWork.FormFieldRepository.GetFieldsByTabIdAsync(tabId);
        }

        public async Task<bool> UpdateFieldAsync(FORM_FIELDS fieldEntity)
        {
            if (fieldEntity == null)
                throw new ArgumentNullException(nameof(fieldEntity));

            // 1. التحقق من وجود الحقل
            var existingField = await GetFieldByIdAsync(fieldEntity.id);
            if (existingField == null)
                throw new InvalidOperationException($"Field with ID '{fieldEntity.id}' does not exist.");

            // 2. التحقق من تكرار FieldCode (باستثناء الحقل الحالي)
            if (!await IsFieldCodeUniqueAsync(fieldEntity.FieldCode, fieldEntity.id))
                throw new InvalidOperationException($"Field code '{fieldEntity.FieldCode}' is already in use by another field.");

            // 3. التحقق من تكرار FieldName (باستثناء الحقل الحالي)
            if (!await IsFieldNameUniqueAsync(fieldEntity.FieldName, fieldEntity.id, fieldEntity.TabId))
                throw new InvalidOperationException($"Field name '{fieldEntity.FieldName}' is already in use in this tab.");

            // 4. تحديث الخصائص
            existingField.FieldName = fieldEntity.FieldName;
            existingField.FieldCode = fieldEntity.FieldCode;
            existingField.FieldOrder = fieldEntity.FieldOrder;
            existingField.Placeholder = fieldEntity.Placeholder;
            existingField.HintText = fieldEntity.HintText;
            existingField.IsMandatory = fieldEntity.IsMandatory;
            existingField.IsEditable = fieldEntity.IsEditable;
            existingField.IsVisible = fieldEntity.IsVisible;
            existingField.DefaultValueJson = fieldEntity.DefaultValueJson;
            existingField.DataType = fieldEntity.DataType;
            existingField.MaxLength = fieldEntity.MaxLength;
            existingField.MinValue = fieldEntity.MinValue;
            existingField.MaxValue = fieldEntity.MaxValue;
            existingField.RegexPattern = fieldEntity.RegexPattern;
            existingField.ValidationMessage = fieldEntity.ValidationMessage;
            existingField.VisibilityRuleJson = fieldEntity.VisibilityRuleJson;
            existingField.ReadOnlyRuleJson = fieldEntity.ReadOnlyRuleJson;
            existingField.FieldTypeId = fieldEntity.FieldTypeId;
            existingField.UpdatedDate = DateTime.UtcNow;
            existingField.IsActive = fieldEntity.IsActive;

            // 5. تحديث الحقل وحفظ التغييرات
            _unitOfWork.Repositary<FORM_FIELDS>().Update(existingField);
            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }

        public async Task<bool> DeleteFieldAsync(int id)
        {
            var fieldToDelete = await GetFieldByIdAsync(id);
            if (fieldToDelete == null)
                return false;

            _unitOfWork.Repositary<FORM_FIELDS>().Delete(fieldToDelete);
            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }

        public async Task<bool> IsFieldCodeUniqueAsync(string fieldCode, int? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(fieldCode))
                return false;

            return await _unitOfWork.FormFieldRepository.IsFieldCodeUniqueAsync(fieldCode, ignoreId);
        }

        public async Task<bool> IsFieldNameUniqueAsync(string fieldName, int? ignoreId = null, int? tabId = null)
        {
            if (string.IsNullOrWhiteSpace(fieldName))
                return false;

            return await _unitOfWork.FormFieldRepository.IsFieldNameUniqueAsync(fieldName, ignoreId, tabId);
        }

        // أو استخدم الـ Repository المتخصص مباشرة
        public async Task<FORM_FIELDS?> GetFieldWithDetailsAsync(int id, bool asNoTracking = false)
        {
            var query = _unitOfWork.FormFieldRepository.GetAll()
                .Where(f => f.id == id)
                .Include(f => f.FORM_TABS)
                .Include(f => f.FIELD_TYPES)
                .Include(f => f.FIELD_OPTIONS);

          

            return await query.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<FORM_FIELDS>> GetAllFieldsAsync(Expression<Func<FORM_FIELDS, bool>>? filter = null)
        {
            var query = _unitOfWork.Repositary<FORM_FIELDS>().GetAll();

            if (filter != null)
                query = query.Where(filter);

            return await query.ToListAsync();
        }

        public async Task<bool> FieldExistsAsync(int id)
        {
            return await _unitOfWork.Repositary<FORM_FIELDS>()
                .AnyAsync(f => f.id == id);
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetActiveFieldsAsync()
        {
            return await _unitOfWork.Repositary<FORM_FIELDS>()
                .GetAllAsync(f => f.IsActive);
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByFormIdAsync(int formBuilderId)
        {
            return await _unitOfWork.FormFieldRepository.GetFieldsByFormIdAsync(formBuilderId);
        }

        public async Task<int> GetFieldsCountAsync(int tabId)
        {
            return await _unitOfWork.FormFieldRepository.GetFieldsCountByTabAsync(tabId);
        }

        public async Task<bool> SoftDeleteFieldAsync(int id)
        {
            var field = await GetFieldByIdAsync(id);
            if (field == null)
                return false;

            field.IsActive = false;
            field.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Repositary<FORM_FIELDS>().Update(field);
            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetMandatoryFieldsAsync(int tabId)
        {
            return await _unitOfWork.FormFieldRepository.GetMandatoryFieldsAsync(tabId);
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetVisibleFieldsAsync(int tabId)
        {
            return await _unitOfWork.FormFieldRepository.GetVisibleFieldsAsync(tabId);
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByDataTypeAsync(string dataType)
        {
            return await _unitOfWork.FormFieldRepository.GetFieldsByDataTypeAsync(dataType);
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsByFieldTypeAsync(int fieldTypeId)
        {
            return await _unitOfWork.FormFieldRepository.GetFieldsByFieldTypeAsync(fieldTypeId);
        }

        public async Task<bool> UpdateFieldsOrderAsync(Dictionary<int, int> fieldOrders)
        {
            return await _unitOfWork.FormFieldRepository.UpdateFieldsOrderAsync(fieldOrders);
        }

        public async Task<IEnumerable<FORM_FIELDS>> GetFieldsWithValidationRulesAsync(int tabId)
        {
            return await _unitOfWork.FormFieldRepository.GetFieldsWithValidationRulesAsync(tabId);
        }
    }
}