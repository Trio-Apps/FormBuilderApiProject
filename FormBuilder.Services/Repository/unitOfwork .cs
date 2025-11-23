using formBuilder.Domian.Entitys;
using formBuilder.Domian.Interfaces;
using FormBuilder.API.Data;

namespace FormBuilder.core.Repository
{
    public class UnitOfWork : IunitOfwork, IAsyncDisposable
    {
        private readonly Dictionary<Type, object> _repositories;

        public FormBuilderDbContext AppDbContext { get; }

        public UnitOfWork(FormBuilderDbContext appDbContext)
        {
            _repositories = new Dictionary<Type, object>();
            AppDbContext = appDbContext;
        }

        // SaveChangesAsync
        public async Task<int> CompleteAsyn()
        {
            return await AppDbContext.SaveChangesAsync();
        }

        // Dispose Context
        public async ValueTask DisposeAsync()
        {
            await AppDbContext.DisposeAsync();
        }

        // Repository Factory
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

      
        
    }
}
