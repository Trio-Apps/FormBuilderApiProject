using formBuilder.Domian.Interfaces;
using FormBuilder.Domian.Entitys.froms;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.Domain.Interfaces.Repositories
{
    public interface IFormSubmissionRepository : IBaseRepository<FORM_SUBMISSIONS>
    {
        Task<IEnumerable<FORM_SUBMISSIONS>> GetByFormBuilderIdAsync(int formBuilderId);
        Task<IEnumerable<FORM_SUBMISSIONS>> GetByUserIdAsync(string userId);
        Task<IEnumerable<FORM_SUBMISSIONS>> GetByStatusAsync(string status);
        Task<bool> ExistsAsync(int id);
        Task<int> GetSubmissionsCountAsync(int formBuilderId);
        Task<FORM_SUBMISSIONS> GetByDocumentNumberAsync(string documentNumber);
        Task<FORM_SUBMISSIONS> GetByIdAsync(int id);

    }
}