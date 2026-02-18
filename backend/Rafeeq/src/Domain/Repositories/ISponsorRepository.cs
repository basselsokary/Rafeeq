using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Domain.Repositories;

public interface ISponsorRepository : IBaseRepository<Sponsor>
{
    Task<IEnumerable<Sponsor>> GetByTypeAsync(SponsorType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sponsor>> GetByTierAsync(SponsorTier tier, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sponsor>> GetActiveSponsorsAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Sponsor>> GetNearbyAsync(GeoLocation location, double radiusKm, CancellationToken cancellationToken = default);
    Task<IEnumerable<Sponsor>> GetWithActiveOffersAsync(CancellationToken cancellationToken = default);
}
