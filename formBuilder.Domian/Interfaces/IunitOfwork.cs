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
        // ... (العمليات الأساسية) ...
        Task<int> CompleteAsyn();
        IBaseRepository<T> Repositary<T>() where T : BaseEntity;

        // ... (المستودعات المحددة) ...
        IFormBuilderRepository FormBuilderRepository { get; }
        IFormTabRepository FormTabRepository { get; }

        // 🆕 جديد: خاصية المستودع الخاص بالحقول
        //IFormFieldRepository IFormBuilderRepository { get; }
        IFormFieldRepository FormFieldRepository { get; } // ✅ التصحيح هنا
        IFieldTypesRepository FieldTypesRepository { get; } // ✅ التصحيح هنا
        IFORM_RULESRepository FORM_RULESRepository { get; }

    }
}