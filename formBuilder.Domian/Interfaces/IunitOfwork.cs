using formBuilder.Domian.Entitys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace formBuilder.Domian.Interfaces
{
    public interface IunitOfwork : IAsyncDisposable
    {
        IBaseRepository<T> Repositary<T>() where T : BaseEntity;
        Task<int> CompleteAsyn();

    }
}
