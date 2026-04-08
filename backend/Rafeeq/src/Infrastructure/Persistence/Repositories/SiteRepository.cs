using Microsoft.EntityFrameworkCore;
using Domain.Entities.SiteAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal class SiteRepository(ApplicationDbContext context)
    : BaseRepository<Site>(context), ISiteRepository
{
    public async Task<Site?> GetWithFacilitiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Facilities)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Site?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.Images)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Site?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.LocalizedContents)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Site?> GetWithNearestTransportationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.NearestTransportations)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Site?> GetWithOpeningHoursAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(s => s.OpeningHours)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
