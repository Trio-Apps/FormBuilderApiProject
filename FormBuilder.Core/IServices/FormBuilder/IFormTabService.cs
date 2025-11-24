using FormBuilder.API.Models;
using FormBuilder.Domian.Entitys.froms; // المسار لكيان FORM_TABS
using System.Collections.Generic;
using System.Threading.Tasks;

// افترض أن هذا هو المسار المناسب لخدماتك
namespace FormBuilder.Core.IServices.FormBuilder.Services.Services
{
    public interface IFormTabService
    {
        /// <summary>
        /// ينشئ تبويباً جديداً في قاعدة البيانات.
        /// </summary>
        Task<FORM_TABS> CreateTabAsync(FORM_TABS tabEntity);

        /// <summary>
        /// يجلب تبويباً واحداً باستخدام معرّفه الفريد.
        /// </summary>
        Task<FORM_TABS> GetTabByIdAsync(int id, bool asNoTracking = false);

        /// <summary>
        /// يجلب جميع التبويبات المرتبطة بنموذج أب محدد (FormBuilder).
        /// </summary>
        Task<IEnumerable<FORM_TABS>> GetTabsByFormIdAsync(int formBuilderId);

        /// <summary>
        /// يقوم بتحديث بيانات تبويب موجود.
        /// </summary>
        Task<bool> UpdateTabAsync(FORM_TABS tabEntity);

        /// <summary>
        /// يحذف تبويباً باستخدام معرّفه.
        /// </summary>
        Task<bool> DeleteTabAsync(int id);

        /// <summary>
        /// يتحقق مما إذا كان TabCode فريداً (مع السماح باستثناء تبويب معين أثناء التحديث).
        /// </summary>
        Task<bool> IsTabCodeUniqueAsync(string tabCode, int? ignoreId = null);
    }
}