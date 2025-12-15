using FormBuilder.Infrastructure.Data;
using FormBuilder.core;
using FormBuilder.Domian.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FormBuilder.Domian.Entitys.FormBuilder;


// افترض أنك تستخدم هذا المسار لطبقة البنية التحتية
namespace FormBuilder.Infrastructure.Repository
    {
        // يجب أن يرث من تطبيق القاعدة العامة (BaseRepository) والواجهة المخصصة
        public class FormTabRepository : BaseRepository<Domian.Entitys.FormBuilder.FORM_TABS>, IFormTabRepository
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
            /// التحقق مما إذا كان TabCode فريداً (غير مستخدم من قبل).
            /// يعيد true إذا كان الكود متاحاً / غير موجود في الجدول، و false إذا كان مستخدماً.
            /// </summary>
            public async Task<bool> IsTabCodeUniqueAsync(string tabCode, int? excludeId = null)
            {
                if (string.IsNullOrEmpty(tabCode))
                {
                    // نعتبره غير صالح => ليس فريداً
                    return false;
                }

                // يبدأ الاستعلام بالبحث عن أي سجل يطابق TabCode
                var query = _context.FORM_TABS.Where(t => t.TabCode == tabCode);

                // إذا تم توفير excludeId (لعمليات التحديث)، يتم استبعاد هذا المعرف من البحث
                if (excludeId.HasValue)
                {
                    query = query.Where(t => t.Id != excludeId.Value);
                }

                // إذا وُجد أي سجل يطابق الشروط فهذا يعني أن الكود "غير فريد"
                var exists = await query.AnyAsync();

                // نعكس النتيجة لأن اسم الدالة يشير إلى "فريد"
                return !exists;
            }
        }
    }

