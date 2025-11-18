using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FormBuilder.core
{
    using formBuilder.Domian.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq.Expressions;

    public partial class BaseRepository<T, TContext> : IBaseRepository<T>
        where T : class
        where TContext : DbContext
    {
        private readonly TContext _db;
        private readonly DbSet<T> _entity;

        public BaseRepository(TContext db)
        {
            _db = db;
            _entity = _db.Set<T>();
        }

        #region CRUD Operations
        public void Add(T entity) => _entity.Add(entity);
        public void AddRange(ICollection<T> entities) => _entity.AddRange(entities);
        public T Update(T entity)
        {
            _db.Update(entity);
            return entity;
        }
        public void UpdateRange(ICollection<T> entities) => _entity.UpdateRange(entities);
        public void Delete(T entity) => _entity.Remove(entity);
        public void DeleteRange(ICollection<T> entities) => _entity.RemoveRange(entities);
        #endregion
        #region GetAll Methods
        // return Query
        public IQueryable<T> GetAll() => _entity;
        public IQueryable<T> GetAll(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var query = _entity.AsQueryable();
            if (filter != null) query = query.Where(filter);
            return ApplyIncludes(query, includes);
        }
        // return List
        public async Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, params Expression<Func<T, object>>[] includes)
        {
            var query = _entity.AsQueryable();
            if (filter != null) query = query.Where(filter);
            query = ApplyIncludes(query, includes);
            return await query.ToListAsync();
        }
        #endregion
        #region Count Methods
        public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
        {
            var query = _entity.AsQueryable();
            if (filter != null) query = query.Where(filter);
            return await query.CountAsync();
        }
        #endregion
        #region Single or Default
        public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false, params Expression<Func<T, object>>[] includes)
        {
            var query = _entity.AsQueryable();
            if (filter != null) query = query.Where(filter);
            query = ApplyIncludes(query, includes);
            if (asNoTracking) query = query.AsNoTracking();
            return await query.FirstOrDefaultAsync();
        }
        #endregion
        #region Any Data
        public async Task<bool> AnyAsync(Expression<Func<T, bool>>? filter = null)
        {
            IQueryable<T> query = _entity.AsNoTracking();
            if (filter != null) query = query.Where(filter);
            return await query.AnyAsync();
        }

        #endregion
    }

    public partial class BaseRepository<T, TContext>
    {
        private IQueryable<T> ApplyIncludes(IQueryable<T> query, params Expression<Func<T, object>>[] includes)
        {
            if (includes == null || !includes.Any()) return query;
            foreach (var include in includes) query = query.Include(include);
            return query;
        }
    }

    // Convenience aliases for backward compatibility
    public class BaseRepository<T> : BaseRepository<T, AppDbContext> where T : class
    {
        public BaseRepository(AppDbContext db) : base(db) { }
    }
}
