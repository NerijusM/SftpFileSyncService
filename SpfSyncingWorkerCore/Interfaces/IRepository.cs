using System.Linq.Expressions;

namespace SpfSyncingWorkerCore.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    TEntity? Get(int id);
    Task<TEntity?> GetAsync(int id);

    List<TEntity> GetAll();
    Task<List<TEntity>> GetAllAsync();

    List<TEntity> Find(Expression<Func<TEntity, bool>> predicate);
    Task<List<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);

    TEntity? SingleOrDefault(Expression<Func<TEntity, bool>> predicate);
    Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

    void Add(TEntity entity);
    Task AddAsync(TEntity entity);

    void AddRange(IEnumerable<TEntity> entities);
    Task AddRangeAsync(IEnumerable<TEntity> entities);

    void Update(TEntity entity);
    Task UpdateAsync(TEntity entity);

    void Delete(TEntity entity);
    Task DeleteAsync(TEntity entity);

    void DeleteRange(IEnumerable<TEntity> entities);
    Task DeleteRangeAsync(IEnumerable<TEntity> entities);
}