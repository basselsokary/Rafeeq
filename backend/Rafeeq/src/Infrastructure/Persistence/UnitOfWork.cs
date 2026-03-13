using RafeeqApp.Domain.Common.Interfaces;
using RafeeqApp.Domain.Repositories;
using RafeeqApp.Infrastructure.Persistence.ApplicationDbContext;
using Microsoft.EntityFrameworkCore.Storage;

namespace RafeeqApp.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _currentTransaction;

    public UnitOfWork(
        ApplicationDbContext context,
        ISiteRepository sites,
        IUserRepository users,
        IReviewRepository reviews,
        ISponsorRepository sponsors,
        ICityRepository cities,
        IContentReportRepository contentReports)
    {
        _context = context;
        Sites = sites;
        Users = users;
        Reviews = reviews;
        Sponsors = sponsors;
        Cities = cities;
        ContentReports = contentReports;
    }

    public ISiteRepository Sites { get; }
    public IUserRepository Users { get; }
    public IReviewRepository Reviews { get; }
    public ISponsorRepository Sponsors { get; }
    public ICityRepository Cities { get; }
    public IContentReportRepository ContentReports { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_currentTransaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _currentTransaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await SaveChangesAsync(cancellationToken);

            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync(cancellationToken);
            }
        }
        finally
        {
            if (_currentTransaction != null)
            {
                _currentTransaction.Dispose();
                _currentTransaction = null;
            }
        }
    }

    public void Dispose()
    {
        _currentTransaction?.Dispose();
        _context.Dispose();
    }
}
