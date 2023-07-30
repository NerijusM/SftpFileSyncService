using Microsoft.EntityFrameworkCore;
using SpfSyncingWorkerCore.Interfaces;
using System.Linq.Expressions;

namespace SpfSyncingWorkerInfrastructure.Data.Repositories
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        public DbContext Context { get; private set; }

        protected Repository(DbContext context)
        {
            Context = context;
        }

        public TEntity? Get(int id)
        {
            return Context.Set<TEntity>().Find(id);
        }

        public Task<TEntity?> GetAsync(int id)
        {
            return Context.Set<TEntity>().FindAsync(id).AsTask();
        }

        public List<TEntity> GetAll()
        {
            return Context.Set<TEntity>().ToList();
        }

        public Task<List<TEntity>> GetAllAsync()
        {
            return Context.Set<TEntity>().ToListAsync();
        }

        public List<TEntity> Find(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).ToList();
        }

        public Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().Where(predicate).ToListAsync();
        }

        public TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefault(predicate);
        }

        public Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return Context.Set<TEntity>().SingleOrDefaultAsync(predicate);
        }

        public void Add(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
            Context.SaveChanges();
        }

        public Task AddAsync(TEntity entity)
        {
            Context.Set<TEntity>().Add(entity);
            return Context.SaveChangesAsync();
        }

        public void AddRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
            Context.SaveChanges();
        }

        public Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().AddRange(entities);
            return Context.SaveChangesAsync();
        }

        public void Update(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            Context.SaveChanges();
        }

        public Task UpdateAsync(TEntity entity)
        {
            Context.Entry(entity).State = EntityState.Modified;
            return Context.SaveChangesAsync();
        }

        public void Delete(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            Context.SaveChanges();
        }

        public Task DeleteAsync(TEntity entity)
        {
            Context.Set<TEntity>().Remove(entity);
            return Context.SaveChangesAsync();
        }

        public void DeleteRange(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            Context.SaveChanges();
        }

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            Context.Set<TEntity>().RemoveRange(entities);
            return Context.SaveChangesAsync();
        }

    }
}
