using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.TouristAggregate;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal sealed class TouristRepository(ApplicationDbContext context)
    : BaseRepository<Tourist>(context), ITouristRepository
{
    public Task<VisitedSite?> GetVisitedSiteAsync(Guid touristId, Guid siteId, CancellationToken cancellationToken = default)
    {
        return DbContext.VisitedSites
            .FirstOrDefaultAsync(vs => vs.TouristId == touristId && vs.SiteId == siteId, cancellationToken);
    }


    public async Task<Tourist?> GetWithFavouritesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(u => u.Favourites)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

    public Task<Tourist?> GetWithVisitedSitesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(u => u.VisitedSites)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

}