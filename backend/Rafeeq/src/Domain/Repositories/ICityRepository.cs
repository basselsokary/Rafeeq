using Domain.Entities.CityAggregate;

namespace Domain.Repositories;

public interface ICityRepository : IBaseRepository<City>
{
    Task<City?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<City>> GetAllWithLocalizedContentsAsync(CancellationToken cancellationToken = default);
}
