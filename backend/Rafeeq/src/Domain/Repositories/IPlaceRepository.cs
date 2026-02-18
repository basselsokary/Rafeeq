using Domain.Entities.AttractionAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface IAttractionRepository : IBaseRepository<Attraction>
{
    Task<IEnumerable<Attraction>> GetByTypeAsync(AttractionType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attraction>> GetByTypeAndCityAsync(AttractionType type, Guid cityId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attraction>> GetNearbyAsync(GeoLocation location, double radiusKm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attraction>> GetPopularAsync(int count, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attraction>> GetFeaturedAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Attraction>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attraction>> GetByStatusAsync(AttractionStatus status, CancellationToken cancellationToken = default);
}
