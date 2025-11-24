using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Data;
using FormBuilder.core;
using FormBuilder.Domain.Interfaces;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Domian.Interfaces;
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

        // الحقول الداعمة (Backing fields) - تصحيح الأسماء
        private IFormBuilderRepository _formBuilderRepository;
        private IFormTabRepository _formTabRepository;
        private IFormFieldRepository _formFieldRepository; // ✅ إضافة FormFieldRepository

        public FormBuilderDbContext AppDbContext { get; }

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

        // --- Specific Repository Exposure ---

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
    }
}