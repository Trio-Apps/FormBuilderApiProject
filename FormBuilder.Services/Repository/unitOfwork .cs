using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domain.Interfaces.Repositories;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Domian.Interfaces;
using FormBuilder.Infrastructure.Repositories;
using FormBuilder.Infrastructure.Repository;
using FormBuilder.Services.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.core.Repository
{
    public class UnitOfWork : IunitOfwork, IAsyncDisposable
    {
        private readonly Dictionary<Type, object> _repositories;

        // Private repository fields
        private IFORM_RULESRepository _formRulesRepository;
        private IFormBuilderRepository _formBuilderRepository;
        private IFormTabRepository _formTabRepository;
        private IFormFieldRepository _formFieldRepository;
        private IFieldTypesRepository _fieldTypesRepository;
        private IFieldOptionsRepository _FieldOptionsRepository;
        private IFieldDataSourcesRepository _fieldDataSourcesRepository;
        private IAttachmentTypeRepository _attachmentTypeRepository;
        private IFormAttachmentTypeRepository _formAttachmentTypeRepository;
        private IDocumentTypeRepository _documentTypeRepository;
        private IDocumentSeriesRepository _documentSeriesRepository;
        private IProjectRepository _projectRepository;
        private IFormSubmissionsRepository _formSubmissionsRepository; // تم التصحيح
        private IFormGridColumnRepository _formGridColumnRepository;
        public FormBuilderDbContext AppDbContext { get; }
        private IFormSubmissionGridRowRepository _formSubmissionGridRowRepository;
        private IFormSubmissionGridCellRepository _formSubmissionGridCellRepository;
        private IFormulasRepository _formulasRepository;

        public UnitOfWork(FormBuilderDbContext appDbContext)
        {
            _repositories = new Dictionary<Type, object>();
            AppDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        // --- Core UoW Methods ---

        public async Task<int> CompleteAsyn()
        {
            return await AppDbContext.SaveChangesAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await AppDbContext.DisposeAsync();
        }

        public IBaseRepository<T> Repositary<T>() where T : BaseEntity
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repo = new BaseRepository<T>(AppDbContext);
                _repositories.Add(type, repo);
            }
            return (IBaseRepository<T>)_repositories[type];
        }

        // --- Specific Repository Properties ---

        public IFORM_RULESRepository FORM_RULESRepository
        {
            get
            {
                _formRulesRepository ??= new FORM_RULESRepository(AppDbContext);
                return _formRulesRepository;
            }
        }

        public IFormBuilderRepository FormBuilderRepository
        {
            get
            {
                _formBuilderRepository ??= new FormBuilderRepository(AppDbContext);
                return _formBuilderRepository;
            }
        }

        public IFormTabRepository FormTabRepository
        {
            get
            {
                _formTabRepository ??= new FormTabRepository(AppDbContext);
                return _formTabRepository;
            }
        }

        public IFormFieldRepository FormFieldRepository
        {
            get
            {
                _formFieldRepository ??= new FormFieldRepository(AppDbContext);
                return _formFieldRepository;
            }
        }

        public IFieldTypesRepository FieldTypesRepository
        {
            get
            {
                _fieldTypesRepository ??= new FieldTypesRepository(AppDbContext);
                return _fieldTypesRepository;
            }
        }

        public IFieldOptionsRepository FieldOptionsRepository
        {
            get
            {
                _FieldOptionsRepository ??= new FieldOptionsRepository(AppDbContext);
                return _FieldOptionsRepository;
            }
        }

        public IFieldDataSourcesRepository FieldDataSourcesRepository
        {
            get
            {
                _fieldDataSourcesRepository ??= new FieldDataSourcesRepository(AppDbContext);
                return _fieldDataSourcesRepository;
            }
        }

        public IAttachmentTypeRepository AttachmentTypeRepository
        {
            get
            {
                _attachmentTypeRepository ??= new AttachmentTypeRepository(AppDbContext);
                return _attachmentTypeRepository;
            }
        }

        public IFormAttachmentTypeRepository FormAttachmentTypeRepository
        {
            get
            {
                _formAttachmentTypeRepository ??= new FormAttachmentTypeRepository(AppDbContext);
                return _formAttachmentTypeRepository;
            }
        }

        public IDocumentTypeRepository DocumentTypeRepository
        {
            get
            {
                _documentTypeRepository ??= new DocumentTypeRepository(AppDbContext);
                return _documentTypeRepository;
            }
        }

        public IProjectRepository ProjectRepository
        {
            get
            {
                _projectRepository ??= new ProjectRepository(AppDbContext);
                return _projectRepository;
            }
        }

        public IDocumentSeriesRepository DocumentSeriesRepository
        {
            get
            {
                _documentSeriesRepository ??= new DocumentSeriesRepository(AppDbContext);
                return _documentSeriesRepository;
            }
        }

        public IFormSubmissionsRepository FormSubmissionsRepository // تم التصحيح
        {
            get
            {
                _formSubmissionsRepository ??= new FormSubmissionsRepository(AppDbContext);
                return _formSubmissionsRepository;
            }
        }
        private IFormSubmissionValuesRepository _formSubmissionValuesRepository;
        public IFormSubmissionValuesRepository FormSubmissionValuesRepository =>
            _formSubmissionValuesRepository ??= new FormSubmissionValuesRepository(AppDbContext);
        private IFormSubmissionAttachmentsRepository _formSubmissionAttachmentsRepository;
        public IFormSubmissionAttachmentsRepository FormSubmissionAttachmentsRepository =>
            _formSubmissionAttachmentsRepository ??= new FormSubmissionAttachmentsRepository(AppDbContext);
        // Add to UnitOfWork class
        private IFormGridRepository _formGridRepository;

        public IFormGridRepository FormGridRepository
        {
            get
            {
                _formGridRepository ??= new FormGridRepository(AppDbContext);
                return _formGridRepository;
            }
        }

        public IFormGridColumnRepository FormGridColumnRepository
        {
            get
            {
                _formGridColumnRepository ??= new FormGridColumnRepository(AppDbContext);
                return _formGridColumnRepository;
            }
        }

        public IFormSubmissionGridRowRepository FormSubmissionGridRowRepository
        {
            get
            {
                _formSubmissionGridRowRepository ??= new FormSubmissionGridRowRepository(AppDbContext);
                return _formSubmissionGridRowRepository;
            }
        }
        public IFormSubmissionGridCellRepository FormSubmissionGridCellRepository =>
                _formSubmissionGridCellRepository ??= new FormSubmissionGridCellRepository(AppDbContext);
        public IFormulasRepository FormulasRepository
        {
            get
            {
                _formulasRepository ??= new FormulasRepository(AppDbContext);
                return _formulasRepository;
            }
        }
        private IFormulaVariablesRepository _formulaVariablesRepository;
        public IFormulaVariablesRepository FormulaVariablesRepository
        {
            get
            {
                if (_formulaVariablesRepository == null)
                {
                    _formulaVariablesRepository = new FormulaVariablesRepository(AppDbContext);
                }
                return _formulaVariablesRepository;
            }
        }

    }
}