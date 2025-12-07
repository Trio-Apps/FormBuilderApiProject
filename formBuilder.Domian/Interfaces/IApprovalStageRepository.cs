using FormBuilder.Domian.Entitys.FormBuilder;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Repositories
{
    public interface IApprovalStageRepository
    {
        Task<IEnumerable<APPROVAL_STAGES>> GetAllAsync();
        Task<APPROVAL_STAGES> GetByIdAsync(int id);
        void Add(APPROVAL_STAGES entity);
        void Update(APPROVAL_STAGES entity);
        void Delete(APPROVAL_STAGES entity);
        Task<bool> AnyAsync(int id);
    }
}
