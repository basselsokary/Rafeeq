using Domain.Common;
using Domain.Common.Interfaces;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Infrastructure.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace Infrastructure.Persistence;

internal sealed class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
{
    private bool _disposed = false;

    private ITouristRepository? _userRepository;
    private ISiteRepository? _siteRepository;
    private IReviewRepository? _reviewRepository;
    private ISponsorRepository? _sponsorRepository;
    private ICityRepository? _cityRepository;
    private IContentReportRepository? _contentReportRepository;
    private IAttractionRepository? _attractionRepository;
    private ITripRepository? _tripRepository;
    
    public ITouristRepository Tourists
        => _userRepository ??= new TouristRepository(context);
    public ISiteRepository Sites
        => _siteRepository ??= new SiteRepository(context);
    public IReviewRepository Reviews
        => _reviewRepository ??= new ReviewRepository(context);
    public ISponsorRepository Sponsors
        => _sponsorRepository ??= new SponsorRepository(context);
    public ICityRepository Cities
        => _cityRepository ??= new CityRepository(context);
    public IContentReportRepository ContentReports
        => _contentReportRepository ??= new ContentReportRepository(context);
    public IAttractionRepository Attractions
        => _attractionRepository ??= new AttractionRepository(context);
    public ITripRepository Trips
        => _tripRepository ??= new TripRepository(context);

    public Task UpdateAsync<T>(T entity) where T : BaseEntity
    {
        context.Update(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync<T>(T entity) where T : BaseEntity
    {
        context.Remove(entity);
        return Task.CompletedTask;
    }

    public async Task AddAsync<T>(T entity, CancellationToken cancellationToken = default) where T : BaseEntity
    {
        await context.AddAsync(entity, cancellationToken);
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ExecuteInTransactionAsync(
        Func<Task> operation,
        CancellationToken cancellationToken = default)
    {
        await ExecuteInTransactionAsync(async () =>
        {
            await operation();
            return true; // dummy result
        }, cancellationToken);
    }
    
    public async Task<TResult> ExecuteInTransactionAsync<TResult>(
        Func<Task<TResult>> operation,
        CancellationToken cancellationToken = default)
    {
        var strategy = context.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            await using var transaction =
                await context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var result = await operation();
                
                if (result is Result { Failed: true })
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return result;
                }

                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return result;
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            context.Dispose();
            _disposed = true;
        }
    }
}
