using formBuilder.Domian.Interfaces;
using FormBuilder.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public class FormTabService : IFormTabService
    {
        private readonly IunitOfwork _unitOfWork;

        public FormTabService(IunitOfwork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<FORM_TABS> CreateTabAsync(FORM_TABS tabEntity)
        {
            if (tabEntity == null)
                throw new ArgumentNullException(nameof(tabEntity));

            // 1. التحقق من وجود النموذج الأب (FormBuilder)
            var formBuilderExists = await _unitOfWork.Repositary<FORM_BUILDER>()
                .SingleOrDefaultAsync(f => f.id == tabEntity.FormBuilderId, asNoTracking: true);

            if (formBuilderExists == null)
            {
                throw new InvalidOperationException($"FormBuilder with ID '{tabEntity.FormBuilderId}' does not exist. Cannot create tab.");
            }

            // 2. التحقق من تكرار TabCode
            if (!await IsTabCodeUniqueAsync(tabEntity.TabCode))
            {
                throw new InvalidOperationException($"Tab code '{tabEntity.TabCode}' is already in use.");
            }

            // 3. تعيين القيم الافتراضية
            if (tabEntity.CreatedDate == default)
                tabEntity.CreatedDate = DateTime.UtcNow;

            tabEntity.IsActive = true;

            // 4. إضافة التبويب وحفظ التغييرات
            _unitOfWork.Repositary<FORM_TABS>().Add(tabEntity);
            await _unitOfWork.CompleteAsyn();

            return tabEntity;
        }

        public async Task<FORM_TABS?> GetTabByIdAsync(int id, bool asNoTracking = false)
        {
            return await _unitOfWork.Repositary<FORM_TABS>()
                .SingleOrDefaultAsync(t => t.id == id, asNoTracking: asNoTracking);
        }

        public async Task<IEnumerable<FORM_TABS>> GetTabsByFormIdAsync(int formBuilderId)
        {
            return await _unitOfWork.Repositary<FORM_TABS>()
                .GetAllAsync(t => t.FormBuilderId == formBuilderId && t.IsActive);
        }

        public async Task<bool> UpdateTabAsync(FORM_TABS tabEntity)
        {
            if (tabEntity == null)
                throw new ArgumentNullException(nameof(tabEntity));

            // 1. التحقق من وجود التبويب
            var existingTab = await GetTabByIdAsync(tabEntity.id);
            if (existingTab == null)
            {
                throw new InvalidOperationException($"Tab with ID '{tabEntity.id}' does not exist.");
            }

            // 2. التحقق من تكرار TabCode (باستثناء التبويب الحالي)
            if (!await IsTabCodeUniqueAsync(tabEntity.TabCode, tabEntity.id))
            {
                throw new InvalidOperationException($"Tab code '{tabEntity.TabCode}' is already in use by another tab.");
            }

            // 3. تحديث الخصائص
            existingTab.TabName = tabEntity.TabName;
            existingTab.TabCode = tabEntity.TabCode;
            existingTab.TabOrder = tabEntity.TabOrder;
            existingTab.IsActive = tabEntity.IsActive;
            existingTab.UpdatedDate = DateTime.UtcNow;

            // 4. تحديث التبويب وحفظ التغييرات
            _unitOfWork.Repositary<FORM_TABS>().Update(existingTab);
            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }

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

        public async Task<bool> IsTabCodeUniqueAsync(string tabCode, int? ignoreId = null)
        {
            if (string.IsNullOrWhiteSpace(tabCode))
                return false;

            var existingTab = await _unitOfWork.Repositary<FORM_TABS>()
                .SingleOrDefaultAsync(t => t.TabCode == tabCode.Trim() &&
                                         (!ignoreId.HasValue || t.id != ignoreId.Value));

            return existingTab == null;
        }

        public async Task<FORM_TABS?> GetTabWithDetailsAsync(int id, bool asNoTracking = false)
        {
            // إذا كنت تريد تحميل العلاقات، يمكنك استخدام Includes
            var tabRepo = _unitOfWork.Repositary<FORM_TABS>();
            return await tabRepo.SingleOrDefaultAsync(
                t => t.id == id,
                asNoTracking: asNoTracking);
            // يمكنك إضافة includes هنا إذا needed
            // includes: t => t.FORM_FIELDS, t => t.FORM_GRIDS
        }

        public async Task<IEnumerable<FORM_TABS>> GetAllTabsAsync(Expression<Func<FORM_TABS, bool>>? filter = null)
        {
            var query = _unitOfWork.Repositary<FORM_TABS>().GetAll();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.ToListAsync();
        }

        public async Task<bool> TabExistsAsync(int id)
        {
            return await _unitOfWork.Repositary<FORM_TABS>()
                .AnyAsync(t => t.id == id);
        }

        // طرق إضافية مفيدة

        public async Task<IEnumerable<FORM_TABS>> GetActiveTabsAsync()
        {
            return await _unitOfWork.Repositary<FORM_TABS>()
                .GetAllAsync(t => t.IsActive);
        }

        public async Task<IEnumerable<FORM_TABS>> GetTabsByFormIdWithDetailsAsync(int formBuilderId)
        {
            return await _unitOfWork.Repositary<FORM_TABS>()
                .GetAllAsync(t => t.FormBuilderId == formBuilderId && t.IsActive);
        }

        public async Task<int> GetTabsCountAsync(int formBuilderId)
        {
            return await _unitOfWork.Repositary<FORM_TABS>()
                .CountAsync(t => t.FormBuilderId == formBuilderId && t.IsActive);
        }

        public async Task<bool> SoftDeleteTabAsync(int id)
        {
            var tab = await GetTabByIdAsync(id);
            if (tab == null)
                return false;

            tab.IsActive = false;
            tab.UpdatedDate = DateTime.UtcNow;

            _unitOfWork.Repositary<FORM_TABS>().Update(tab);
            var result = await _unitOfWork.CompleteAsyn();

            return result > 0;
        }
    }
}