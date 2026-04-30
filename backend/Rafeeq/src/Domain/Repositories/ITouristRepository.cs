using Domain.Entities.TouristAggregate;

namespace Domain.Repositories;

public interface ITouristRepository : IBaseRepository<Tourist>
{
    Task<Tourist?> GetWithFavouritesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Tourist?> GetWithVisitedSitesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<VisitedSite?> GetVisitedSiteAsync(Guid touristId, Guid siteId, CancellationToken cancellationToken = default);
}
