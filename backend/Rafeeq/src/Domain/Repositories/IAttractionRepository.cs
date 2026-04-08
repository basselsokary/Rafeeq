using Domain.Entities.AttractionAggregate;

namespace Domain.Repositories;

public interface IAttractionRepository : IBaseRepository<Attraction>
{
    Task<Attraction?> GetWithImagesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Attraction?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default);
    }
