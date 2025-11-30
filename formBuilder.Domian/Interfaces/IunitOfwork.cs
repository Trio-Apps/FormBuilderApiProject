using formBuilder.Domian.Entitys;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domain.Interfaces.Repositories;
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
        IFieldDataSourcesRepository FieldDataSourcesRepository { get; }
        IAttachmentTypeRepository AttachmentTypeRepository { get; }
        IFormAttachmentTypeRepository FormAttachmentTypeRepository { get; } // Added
        IDocumentTypeRepository DocumentTypeRepository { get; }
        IProjectRepository ProjectRepository { get; }
        IDocumentSeriesRepository DocumentSeriesRepository { get; }
        IFormSubmissionsRepository FormSubmissionsRepository { get; } 
        IFormSubmissionValuesRepository FormSubmissionValuesRepository { get; } 
        IFormSubmissionAttachmentsRepository FormSubmissionAttachmentsRepository { get; } 





    }
}