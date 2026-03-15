using Domain.Common.Interfaces;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Persistence;

internal class UnitOfWork(
    ApplicationDbContext context) : IUnitOfWork
{
    private readonly ApplicationDbContext _context = context;
    private IDbContextTransaction? _currentTransaction;

    private ITouristRepository? _userRepository;
    private ISiteRepository? _siteRepository;
    private IReviewRepository? _reviewRepository;
    private ISponsorRepository? _sponsorRepository;
    private ICityRepository? _cityRepository;
    private IContentReportRepository? _contentReportRepository;
    private IAttractionRepository? _attractionRepository;
    private ITripRepository? _tripRepository;
    
    public ITouristRepository Tourists
        => _userRepository ??= new TouristRepository(_context);
    public ISiteRepository Sites
        => _siteRepository ??= new SiteRepository(_context);
    public IReviewRepository Reviews
        => _reviewRepository ??= new ReviewRepository(_context);
    public ISponsorRepository Sponsors
        => _sponsorRepository ??= new SponsorRepository(_context);
    public ICityRepository Cities
        => _cityRepository ??= new CityRepository(_context);
    public IContentReportRepository ContentReports
        => _contentReportRepository ??= new ContentReportRepository(_context);
    public IAttractionRepository Attractions
        => _attractionRepository ??= new AttractionRepository(_context);
    // public ITripRepository Trips
    //     => _tripRepository ??= new TripRepository(_context);


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
