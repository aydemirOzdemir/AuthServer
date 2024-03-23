using AuthServer.Core.Repositories;
using AuthServer.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private readonly AuthServerDbContext context;
        private readonly DbSet<TEntity> dbSet;

        public GenericRepository(AuthServerDbContext context)
        {
            this.context = context;
            dbSet=context.Set<TEntity>();
        }
        public async Task Add(TEntity entity)
        {
            await dbSet.AddAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync()
        {
           return await dbSet.ToListAsync(); 
        }

        public async Task<TEntity> GetByIdAsync(int id)
        {
            var entity = await dbSet.FindAsync(id);
            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Detached;// Burada gelen nesnenin takip edilmesini istemiyorum.
            }
            return entity;
        }

        public void Remove(TEntity entity)
        {
           dbSet.Remove(entity);
        }

        public TEntity Update(TEntity entity)
        {
            context.Entry(entity).State= EntityState.Modified;
            return entity;
        }

        public IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> predicate)
        {
            return dbSet.Where(predicate);
        }
    }
}
