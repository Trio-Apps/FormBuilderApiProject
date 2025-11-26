using formBuilder.Domian.Entitys;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domian.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formBuilder.Domian.Interfaces
{
    public interface IunitOfwork : IAsyncDisposable
    {
        // Core UoW methods
        Task<int> CompleteAsyn();
        IBaseRepository<T> Repositary<T>() where T : BaseEntity;

        // Specific repositories
        IFormBuilderRepository FormBuilderRepository { get; }
        IFormTabRepository FormTabRepository { get; }
        IFormFieldRepository FormFieldRepository { get; }
        IFieldTypesRepository FieldTypesRepository { get; }
        IFORM_RULESRepository FORM_RULESRepository { get; }
        IFieldOptionsRepository FieldOptionsRepository { get; } 


   
    }
}