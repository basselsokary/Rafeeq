using System.Collections.Concurrent;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.UnitOfWork;
using Domain.Common;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Data.Application.Context;
using Infrastructure.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Repositories;

internal class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;
    private readonly ConcurrentDictionary<Type, object> _repositories;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;
    private readonly IConfiguration _configuration;
    private readonly UserManager<ApplicationUser> _userManager;

    private IUserRepository? _userRepository;
    private IRefreshTokenRepository? _refreshTokenRepository;

    public UnitOfWork(AppDbContext context, IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _configuration = configuration;
        _userManager = userManager;

        _repositories = new ConcurrentDictionary<Type, object>();
    }

    public IUserRepository Users
        => _userRepository ??= new UserRepository(_context);

    public IRefreshTokenRepository RefreshTokens
        => _refreshTokenRepository ??= new RefreshTokenRepository(_context, _configuration, _userManager);

    /// Generic Repository
    public IEntityBaseRepository<T> Repository<T>() where T : BaseEntity
    {
        return (IEntityBaseRepository<T>)_repositories.GetOrAdd(typeof(T),
            _ => new EntityBaseRepository<T>(_context));
    }

    /// Transaction management
    public async Task<IDbContextTransaction> BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        return _transaction;
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to commit.");
        }

        try
        {
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to rollback.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Handle concurrency conflicts
            throw new InvalidOperationException("Concurrency conflict occurred. Please refresh and try again.", ex);
        }
        catch (DbUpdateException ex)
        {
            // Handle database update errors
            throw new InvalidOperationException("Database update failed. Please check your data and try again.", ex);
        }
    }

    /// Dispose pattern
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _transaction?.Dispose();
            _context.Dispose();
            _disposed = true;
        }
    }
}
