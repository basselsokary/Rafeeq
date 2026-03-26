using Domain.Entities.TouristAggregate;

namespace Domain.Repositories;

public interface ITouristRepository : IBaseRepository<Tourist>
{
    Task<Tourist?> GetWithFavouritesAsync(Guid id, CancellationToken cancellationToken = default);
}
