using global::FormBuilder.API.Models;
using global::FormBuilder.Domian.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FormBuilder.Core.IServices.FormBuilder
{
   


    namespace FormBuilder.Services.Services
    {
        public interface IFormBuilderService
        {
            Task<FORM_BUILDER> CreateFormAsync(FORM_BUILDER form);
            Task<FORM_BUILDER> UpdateFormAsync(FORM_BUILDER form);
            Task<bool> DeleteFormAsync(int id);
            Task<FORM_BUILDER?> GetFormByIdAsync(int id, bool asNoTracking = false);
            Task<FORM_BUILDER?> GetFormByCodeAsync(string formCode, bool asNoTracking = false);
            Task<IEnumerable<FORM_BUILDER>> GetAllFormsAsync(Expression<Func<FORM_BUILDER, bool>>? filter = null);
            Task<bool> IsFormCodeExistsAsync(string formCode, int? excludeId = null);
            Task<int> GetFormsCountAsync(Expression<Func<FORM_BUILDER, bool>>? filter = null);
            Task<bool> AnyFormsAsync(Expression<Func<FORM_BUILDER, bool>>? filter = null);
        }

    }
}
