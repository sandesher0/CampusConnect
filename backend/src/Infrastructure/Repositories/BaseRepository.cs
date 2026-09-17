using Microsoft.EntityFrameworkCore;
using SharedKernel.Interfaces;

namespace Infrastructure.Repositories;

public class BaseRepository<TContext, TEntity> : IBaseRepository<TEntity>
where TContext : DbContext
where TEntity : class
{
    protected readonly TContext context;
    protected readonly DbSet<TEntity> dbSet;

    public BaseRepository(TContext context)
    {
        this.context = context;
        dbSet = context.Set<TEntity>();

    }

    public virtual async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await dbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task AddAsync(TEntity entity, CancellationToken cancellationToken)
    {
        await dbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task UpdateAsync(TEntity entity, CancellationToken cancellationToken)
    {
        dbSet.Update(entity);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken)
    {
        dbSet.Remove(entity);
    }
}