using formBuilder.Domian.Interfaces; // واجهة IunitOfwork
using FormBuilder.Core.IServices.FormBuilder.Services.Services; // واجهة IFormTabService
using FormBuilder.Domian.Entitys.froms; // لكيان FORM_TABS
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// افترض أن هذا هو مسار خدماتك (FormBuilder.Services.FormBuilder.Services)
namespace FormBuilder.Services.FormBuilder.Services
{
    public class FormTabService : IFormTabService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormTabService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // --- 1. إنشاء تبويب (Create Tab) ---
        public async Task<FORM_TABS> CreateTabAsync(FORM_TABS tabEntity)
        {
            // 1. التحقق من وجود النموذج الأب (FormBuilder)
            var formBuilderExists = await _unitOfWork.FormBuilderRepository.GetByIdAsync(tabEntity.FormBuilderId, asNoTracking: true);
            if (formBuilderExists == null)
            {
                throw new InvalidOperationException($"FormBuilder with ID '{tabEntity.FormBuilderId}' does not exist. Cannot create tab.");
            }

            // 2. التحقق من تكرار TabCode
            if (await _unitOfWork.FormTabRepository.IsTabCodeUniqueAsync(tabEntity.TabCode))
            {
                throw new InvalidOperationException($"Tab code '{tabEntity.TabCode}' is already in use.");
            }

            // 3. إضافة البيانات وحفظ التغييرات
            _unitOfWork.Repositary<FORM_TABS>().Add(tabEntity);
            await _unitOfWork.CompleteAsyn();

            return tabEntity;
        }

        // --- 2. جلب تبويب حسب المعرف (Get Tab by ID) ---
        public async Task<FORM_TABS> GetTabByIdAsync(int id, bool asNoTracking = false)
        {
            return await _unitOfWork.Repositary<FORM_TABS>().GetByIdAsync(id, asNoTracking);
        }

        // --- 3. جلب التبويبات حسب معرف النموذج الأب (Get Tabs by Form ID) ---
        public async Task<IEnumerable<FORM_TABS>> GetTabsByFormIdAsync(int formBuilderId)
        {
            // استخدام الوظيفة المخصصة في المستودع المحدد (FormTabRepository)
            return await _unitOfWork.FormTabRepository.GetTabsByFormIdAsync(formBuilderId);
        }

        // --- 4. تحديث تبويب (Update Tab) ---
        public async Task<bool> UpdateTabAsync(FORM_TABS tabEntity)
        {
            // 1. التحقق من تكرار TabCode (باستثناء التبويب الحالي)
            // 🟢 تم التصحيح إلى tabEntity.Id
            if (await _unitOfWork.FormTabRepository.IsTabCodeUniqueAsync(tabEntity.TabCode, tabEntity.id))
            {
                throw new InvalidOperationException($"Tab code '{tabEntity.TabCode}' is already in use by another tab.");
            }

            // 2. تحديث الكيان وحفظ التغييرات
            _unitOfWork.Repositary<FORM_TABS>().Update(tabEntity);
            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }

        // --- 5. حذف تبويب (Delete Tab) ---
        public async Task<bool> DeleteTabAsync(int id)
        {
            var tabToDelete = await GetTabByIdAsync(id);
            if (tabToDelete == null)
            {
                return false;
            }

            _unitOfWork.Repositary<FORM_TABS>().Delete(tabToDelete);
            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }

        // --- 6. التحقق من الكود الفريد (Check Unique Code) ---
        public async Task<bool> IsTabCodeUniqueAsync(string tabCode, int? ignoreId = null)
        {
            return await _unitOfWork.FormTabRepository.IsTabCodeUniqueAsync(tabCode, ignoreId);
        }
    }
}