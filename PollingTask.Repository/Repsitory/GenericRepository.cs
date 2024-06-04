using Microsoft.EntityFrameworkCore;
using PollingTask.Repository.IRepsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace PollingTask.Repository.Repsitory
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext context;
        private DbSet<T> entities;

        public GenericRepository(ApplicationDbContext context)
        {
            this.context = context;
            entities = context.Set<T>();
        }

        public async Task<bool> Add(T entity)
        {
            try
            {
                await entities.AddAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        {
            return await context.Set<T>().AnyAsync(predicate);
        }

        public async Task<bool> Delete(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                var result = entities.Remove(entity);
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public IEnumerable<T> GetAll()
        {
            return entities.AsEnumerable();
        }

        public async Task<T> GetById(int Id)
        {
            return await context.Set<T>().FindAsync(Id);
        }

        public async Task<bool> Update(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                var result = entities.Attach(entity);
                result.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                return true;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
    }
}
