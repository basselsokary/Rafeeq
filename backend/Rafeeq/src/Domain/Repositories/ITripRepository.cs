using Domain.Entities.TripAggregate;

namespace Domain.Repositories;

public interface ITripRepository : IBaseRepository<Trip>
{
    Task<Trip?> GetWithDaysAsync(Guid id, CancellationToken cancellationToken = default);
}
