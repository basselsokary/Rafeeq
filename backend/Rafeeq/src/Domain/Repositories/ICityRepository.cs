using Domain.Entities.CityAggregate;

namespace Domain.Repositories;

public interface ICityRepository : IBaseRepository<City>
{
    Task<City?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<City>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
