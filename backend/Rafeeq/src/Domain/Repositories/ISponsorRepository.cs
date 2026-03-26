using Domain.Entities.SponsorAggregate;

namespace Domain.Repositories;

public interface ISponsorRepository : IBaseRepository<Sponsor>
{
    Task<Sponsor?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Sponsor?> GetWithOffersAsync(Guid id, CancellationToken cancellationToken = default);
}
