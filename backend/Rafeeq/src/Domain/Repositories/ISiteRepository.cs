using Domain.Entities.SiteAggregate;

namespace Domain.Repositories;

public interface ISiteRepository : IBaseRepository<Site>
{
    Task<Site?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Site?> GetWithNearestTransportationAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Site?> GetWithFacilitiesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Site?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Site?> GetWithOpeningHoursAsync(Guid id, CancellationToken cancellationToken = default);
}
