using Application.Common.Interfaces.Repositories;
using Domain.Common;
using Infrastructure.Data.Application.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories.Implementations;
internal class EntityBaseRepository<T> : IEntityBaseRepository<T> where T : BaseEntity
{
    protected readonly AppDbContext DbContext;
    private readonly DbSet<T> _dbSet;

    public EntityBaseRepository(AppDbContext _context)
    {
        DbContext = _context;
        _dbSet = DbContext.Set<T>();
    }

    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        var entry = await _dbSet.AddAsync(entity, cancellationToken);
        return entry.Entity;
    }

    public virtual async Task<IEnumerable<T>> AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public virtual Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        DbContext.Entry(entity).CurrentValues.SetValues(entity); // Update only the modified fields
        DbContext.Entry(entity).State = EntityState.Modified;  // Set state as Modified
        return Task.CompletedTask;
    }

    public virtual Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        if (_dbSet.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public virtual async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = await GetByIdAsync(id, cancellationToken: cancellationToken);

        if (entity != null)
        {
            await DeleteAsync(entity, cancellationToken);
        }
    }

    public virtual async Task<T?> GetByIdAsync(Guid id, bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if (asNoTracking)
            return await _dbSet.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, cancellationToken);

        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(bool asNoTracking = false, CancellationToken cancellationToken = default)
    {
        if (asNoTracking)
            return await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

        return await _dbSet.ToListAsync(cancellationToken);
    }

    public Task<T?> GetAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        bool asSplitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = Get(filter, orderBy, asNoTracking, asSplitQuery, includeProperties);
        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<TResult?> GetProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asSplitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = Get(filter, orderBy, true, asSplitQuery, includeProperties);

        if (selector != null)
            return await query.Select(selector).FirstOrDefaultAsync();

        if (typeof(T) != typeof(TResult))
            throw new InvalidOperationException("Cannot cast T to TResult without a selector.");

        // If no selector, cast T to TResult (must be compatible)
        var entity = await query.Cast<TResult>().FirstOrDefaultAsync();
        return entity ?? default(TResult);
    }

    public async Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = false,
        bool asSplitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = Get(filter, orderBy, asNoTracking, asSplitQuery, includeProperties);
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<TResult>> GetAllProjectedAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asSplitQuery = false,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = Get(filter, orderBy, true, asSplitQuery, includeProperties);

        if (selector != null)
            return await query.Select(selector).ToListAsync();

        if (typeof(T) != typeof(TResult))
            throw new InvalidOperationException("Cannot cast T to TResult without a selector.");

        // If no selector, cast T to TResult (must be compatible)
        return await query.Cast<TResult>().ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<TResult> Items, int TotalCount)> GetPagedAsync<TResult>(
        int pageNumber,
        int pageSize,
        Expression<Func<T, bool>>? filter = null,
        Expression<Func<T, TResult>>? selector = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        CancellationToken cancellationToken = default,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet;

        if (includeProperties != null)
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        if (filter != null)
            query = query.Where(filter);

        // Get total count before pagination
        int totalCount = await query.CountAsync(cancellationToken);

        if (orderBy != null)
            query = orderBy(query);

        // Apply pagination
        query = query.Skip((pageNumber - 1) * pageSize).Take(pageSize);

        // Apply projection
        IEnumerable<TResult> items;
        if (selector != null)
        {
            items = await query.Select(selector).ToListAsync(cancellationToken);
        }
        else
        {
            items = await query.Cast<TResult>().ToListAsync(cancellationToken);
        }

        return (items, totalCount);
    }

    public async Task<int> CountAsync(Expression<Func<T, bool>>? filter = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        return await query.CountAsync(cancellationToken);
    }

    public Task<bool> AnyAsync(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
        => _dbSet.AnyAsync(filter, cancellationToken);

    public IQueryable<T> GetQueryable() => _dbSet;

    private IQueryable<T> Get(
        Expression<Func<T, bool>>? filter,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy,
        bool asNoTracking = false,
        bool asSplitQuery = false,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet;

        // Apply As No Tracking
        if (asNoTracking)
            query = query.AsNoTracking();

        // Apply As Split Query
        if (asSplitQuery && includeProperties.Length > 0)
            query = query.AsSplitQuery();

        // Apply includes
        if (includeProperties != null)
            query = includeProperties.Aggregate(query, (current, includeProperty) => current.Include(includeProperty));

        // Apply filter
        if (filter != null)
            query = query.Where(filter);

        // Apply ordering
        if (orderBy != null)
            query = orderBy(query);

        return query;
    }
}