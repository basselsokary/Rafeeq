using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface ISiteRepository : IBaseRepository<Site>
{
    Task<IEnumerable<Site>> GetByTypeAsync(SiteType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Site>> GetByTypeAndCityAsync(SiteType type, Guid cityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Site>> GetNearbyAsync(GeoLocation location, double radiusKm, CancellationToken cancellationToken = default);
    // Task<IEnumerable<Site>> GetPopularAsync(int count, CancellationToken cancellationToken = default);
    // Task<IEnumerable<Site>> GetFeaturedAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Site>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Site>> GetByStatusAsync(SiteStatus status, CancellationToken cancellationToken = default);
}
