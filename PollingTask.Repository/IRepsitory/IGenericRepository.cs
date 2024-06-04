using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PollingTask.Repository.IRepsitory
{
    public interface IGenericRepository<T> where T : class
    {
        Task<bool> Add(T entity);
        Task<bool> Update(T entity);
        Task<bool> Delete(T entity);
        Task<T> GetById(int Id);
        IEnumerable<T> GetAll();
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);



    }
}
