using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace formBuilder.Domian.Interfaces
{
    public interface IBaseRepository<T> where T : class
    {
        #region CRUD Operations
        void Add(T entity);
        void AddRange(ICollection<T> entities);
        T Update(T entity);
        void UpdateRange(ICollection<T> entities);
        void Delete(T entity);
        void DeleteRange(ICollection<T> entities);
        #endregion

        #region GetAll Methods
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes);
        Task<ICollection<T>> GetAllAsync(Expression<Func<T, bool>> filter = null, params Expression<Func<T, object>>[] includes);
        #endregion

        #region Count Methods
        Task<int> CountAsync(Expression<Func<T, bool>> filter = null);
        #endregion

        #region Single or Default
        /// <summary>
        /// 🆕 جديد: يجلب كياناً باستخدام المعرّف الرئيسي (ID).
        /// </summary>
        Task<T> GetByIdAsync(int id, bool asNoTracking = false);

        /// <summary>
        /// 🆕 جديد: يجلب كياناً باستخدام المعرّف ويتيح تضمين علاقات (Includes).
        /// </summary>
        Task<T> GetByIdAsync(int id, bool asNoTracking = false, params Expression<Func<T, object>>[] includes);

        Task<T> SingleOrDefaultAsync(Expression<Func<T, bool>> filter, bool asNoTracking = false, params Expression<Func<T, object>>[] includes);
        #endregion

        #region Any Data
        Task<bool> AnyAsync(Expression<Func<T, bool>> filter = null!);
        #endregion
    }
}