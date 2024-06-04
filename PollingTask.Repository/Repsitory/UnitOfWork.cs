using PollingTask.Repository.IRepsitory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PollingTask.Repository.Repsitory
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext context;
        private readonly Dictionary<Type, object> repostories;

        public UnitOfWork(ApplicationDbContext context)
        {
            this.context = context;
            repostories = new Dictionary<Type, object>();
        }
        public async Task<int> Complete()
          => await context.SaveChangesAsync();


        public void Dispose()
           => context.Dispose();

        public IGenericRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var type = typeof(TEntity);

            if (!repostories.ContainsKey(type))
            {
                var repository = new GenericRepository<TEntity>(context);
                repostories.Add(type, repository);
            }

            return (IGenericRepository<TEntity>)repostories[type];
        }

    }
}
