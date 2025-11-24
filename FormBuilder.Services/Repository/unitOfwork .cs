using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Data; // المسار المفترض لـ DbContext
using FormBuilder.core;
using FormBuilder.Domian.Entitys.froms;
using FormBuilder.Domian.Interfaces;

using FormBuilder.Infrastructure.Repository; // المسار الصحيح لتطبيق FormFieldRepository
using FormBuilder.Services.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FormBuilder.core.Repository
{
    public class UnitOfWork : IunitOfwork, IAsyncDisposable
    {
        private readonly Dictionary<Type, object> _repositories;

        // الحقول الداعمة (Backing fields)
        private IFormBuilderRepository? _formBuilderRepository;
        private IFormTabRepository? _formTabRepository;
        //private IFormFieldRepository? _formFieldRepository; // 🆕 جديد: الحقل الداعم للحقول

        public FormBuilderDbContext AppDbContext { get; }

        public UnitOfWork(FormBuilderDbContext appDbContext)
        {
            _repositories = new Dictionary<Type, object>();
            AppDbContext = appDbContext;
        }

        // --- Core UoW Methods (CompleteAsyn, DisposeAsync, Repositary<T>...) ---

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
                var repo = new BaseRepository<T, FormBuilderDbContext>(AppDbContext);
                _repositories.Add(type, repo);
            }
            return (IBaseRepository<T>)_repositories[type];
        }


        // --- Specific Repository Exposure ---

        public IFormBuilderRepository FormBuilderRepository
        {
            get
            {
                if (_formBuilderRepository == null)
                {
                    _formBuilderRepository = new FormBuilderRepository(AppDbContext);
                }
                return _formBuilderRepository;
            }
        }

        public IFormTabRepository FormTabRepository
        {
            get
            {
                if (_formTabRepository == null)
                {
                    _formTabRepository = new FormTabRepository(AppDbContext);
                }
                return _formTabRepository;
            }
        }

        /// <summary>
        /// 🆕 جديد: يوفر الوصول لعمليات FormField الخاصة.
        /// </summary>
        //public IFormFieldRepository FormFieldRepository
        //{
        //    get
        //    {
        //        // التهيئة الكسولة (Lazy initialization)
        //        if (_formFieldRepository == null)
        //        {
        //            // يجب أن يكون اسم الفئة FormFieldRepository
        //            _formFieldRepository = new FormFieldRepository(AppDbContext);
        //        }
        //        return _formFieldRepository;
        //    }
        //}
    }
}