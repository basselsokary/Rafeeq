using Microsoft.EntityFrameworkCore;
using Domain.Entities.SiteAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Domain.Enums;

namespace Infrastructure.Persistence.Repositories;

internal sealed class SiteRepository(ApplicationDbContext context)
    : BaseRepository<Site>(context), ISiteRepository
{
    public async Task AddNearestTransportationAsync(NearestTransportation transportation, CancellationToken cancellationToken = default)
    {
        await DbContext.NearestTransportations.AddAsync(transportation, cancellationToken);
    }

    public async Task AddNearestTransportationsRangeAsync(IEnumerable<NearestTransportation> transportations, CancellationToken cancellationToken = default)
    {
        await DbContext.NearestTransportations.AddRangeAsync(transportations, cancellationToken);
    }

    public async Task<IEnumerable<Site>> GetAllWithOpeningHoursAsync(CancellationToken ct)
    {
        return await DbSet
            .AsSplitQuery()
            .Include(s => s.LocalizedContents .Where(lc => lc.Language == LanguageCode.English))
            .Include(s => s.OpeningHours)
            .ToListAsync(ct);
    }

    public async Task<NearestTransportation?> GetNearestTransportationByIdAsync(Guid tranportationId, CancellationToken cancellationToken = default)
    {
        return await DbContext.NearestTransportations
            .Include(nt => nt.LocalizedContents)
            .FirstOrDefaultAsync(nt => nt.Id == tranportationId, cancellationToken);
    }

    public async Task<Site?> GetWithFacilitiesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Facilities)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Site?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsSplitQuery()
            .Include(s => s.Images)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Site?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsSplitQuery()
            .Include(s => s.LocalizedContents)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Site?> GetWithNearestTransportationAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsSplitQuery()
            .Include(s => s.NearestTransportations)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Site?> GetWithOpeningHoursAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.OpeningHours)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }
}
