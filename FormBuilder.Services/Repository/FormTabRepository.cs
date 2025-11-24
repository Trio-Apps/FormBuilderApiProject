using FormBuilder.API.Data;
using FormBuilder.core;
using FormBuilder.Domian.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


    // افترض أنك تستخدم هذا المسار لطبقة البنية التحتية
    namespace FormBuilder.Infrastructure.Repository
    {
        // يجب أن يرث من تطبيق القاعدة العامة (BaseRepository) والواجهة المخصصة
        public class FormTabRepository : BaseRepository<FORM_TABS>, IFormTabRepository
        {
            // الوصول إلى DbContext مباشرة لتنفيذ الاستعلامات المعقدة
            private readonly FormBuilderDbContext _context;

            public FormTabRepository(FormBuilderDbContext context) : base(context)
            {
                _context = context;
            }

            // --- تنفيذ الوظائف المخصصة من IFormTabRepository ---

            /// <summary>
            /// استرداد جميع التبويبات المرتبطة بـ FormBuilder محدد.
            /// </summary>
            public async Task<IEnumerable<FORM_TABS>> GetTabsByFormIdAsync(int formBuilderId)
            {
                // يجلب التبويبات التابعة لنموذج معين ويرتبها حسب TabOrder
                return await _context.FORM_TABS
                                     .Where(t => t.FormBuilderId == formBuilderId)
                                     .OrderBy(t => t.TabOrder)
                                     .ToListAsync();
            }

            /// <summary>
            /// للتحقق مما إذا كان TabCode محدداً موجوداً بالفعل.
            /// </summary>
            public async Task<bool> IsTabCodeUniqueAsync(string tabCode, int? excludeId = null)
            {
                if (string.IsNullOrEmpty(tabCode))
                {
                    return false; // لا يمكن التحقق إذا كان الكود فارغاً
                }

                // يبدأ الاستعلام بالبحث عن أي سجل يطابق TabCode
                var query = _context.FORM_TABS.Where(t => t.TabCode == tabCode);

                // إذا تم توفير excludeId (لعمليات التحديث)، يتم استبعاد هذا المعرف من البحث
                if (excludeId.HasValue)
                {
                    query = query.Where(t => t.id != excludeId.Value);
                }

                // التحقق مما إذا كان أي سجل يطابق الشروط موجوداً
                return await query.AnyAsync();
            }
        }
    }

