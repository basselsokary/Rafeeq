using Domain.Common.Interfaces;
using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Common;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal abstract class BaseRepository<TEntity> : IBaseRepository<TEntity>
    where TEntity : BaseEntity, IAggregateRoot
{
    protected readonly ApplicationDbContext DbContext;
    protected readonly DbSet<TEntity> DbSet;

    protected BaseRepository(ApplicationDbContext context)
    {
        DbContext = context;
        DbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IEnumerable<TEntity>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await DbSet.ToListAsync(cancellationToken);
    }

    public virtual async Task AddAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddAsync(entity, cancellationToken);
    }

    public virtual async Task AddRangeAsync(
        IEnumerable<TEntity> entities,
        CancellationToken cancellationToken = default)
    {
        await DbSet.AddRangeAsync(entities, cancellationToken);
    }

    public virtual Task UpdateAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Update(entity);
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(
        TEntity entity,
        CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task<bool> AnyAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(e => e.Id.Equals(id), cancellationToken);
    }
}
