using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface ISponsorRepository : IBaseRepository<Sponsor>
{
    Task<Sponsor?> GetWithImages(Guid id, CancellationToken cancellationToken = default);
    Task<Sponsor?> GetWithOffers(Guid id, CancellationToken cancellationToken = default);
    // Task<IEnumerable<Sponsor>> GetActiveSponsorsAsync(CancellationToken cancellationToken = default);
    // Task<IEnumerable<Sponsor>> GetNearbyAsync(GeoLocation location, double radiusKm, CancellationToken cancellationToken = default);
    // Task<IEnumerable<Sponsor>> GetWithActiveOffersAsync(CancellationToken cancellationToken = default);
}
