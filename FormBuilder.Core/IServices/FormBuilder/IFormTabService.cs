using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Services.Services
{
    public interface IFormTabService
    {
        Task<FORM_TABS> CreateTabAsync(FORM_TABS tabEntity);
        Task<FORM_TABS?> GetTabByIdAsync(int id, bool asNoTracking = false);
        Task<IEnumerable<FORM_TABS>> GetTabsByFormIdAsync(int formBuilderId);
        Task<bool> UpdateTabAsync(FORM_TABS tabEntity);
        Task<bool> DeleteTabAsync(int id);
        Task<bool> IsTabCodeUniqueAsync(string tabCode, int? ignoreId = null);
        Task<FORM_TABS?> GetTabWithDetailsAsync(int id, bool asNoTracking = false);
        Task<IEnumerable<FORM_TABS>> GetAllTabsAsync(Expression<Func<FORM_TABS, bool>>? filter = null);
        Task<bool> TabExistsAsync(int id);
    }
}