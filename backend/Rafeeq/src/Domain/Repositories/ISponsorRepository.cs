using Domain.Entities.SponsorAggregate;

namespace Domain.Repositories;

public interface ISponsorRepository : IBaseRepository<Sponsor>
{
    Task<Sponsor?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sponsor?> GetWithOffersAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sponsor?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Offer?> GetOfferByIdAsync(Guid offerId, CancellationToken cancellationToken = default);
}
