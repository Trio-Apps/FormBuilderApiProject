using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.froms; // المسار المفترض لكيان FORM_TABS
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domian.Interfaces
{
    public interface IFormTabRepository : IBaseRepository<FORM_TABS>
    {
        /// <summary>
        /// للتحقق مما إذا كان TabCode محدداً موجوداً بالفعل، مع استثناء سجل معين (أثناء التحديث).
        /// </summary>
        /// <param name="tabCode">الكود المراد التحقق منه.</param>
        /// <param name="excludeId">المعرف المراد استثناؤه من البحث (لعمليات التحديث).</param>
        Task<bool> IsTabCodeUniqueAsync(string tabCode, int? excludeId = null);

        /// <summary>
        /// استرداد جميع التبويبات المرتبطة بـ FormBuilder محدد باستخدام معرفه.
        /// </summary>
        /// <param name="formBuilderId">معرف النموذج الأب (FormBuilder).</param>
        Task<IEnumerable<FORM_TABS>> GetTabsByFormIdAsync(int formBuilderId);
    }
}