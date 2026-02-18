using Domain.Entities.TripAggregate;
using Domain.Enums;

namespace Domain.Repositories;

public interface ITripRepository : IBaseRepository<Trip>
{
    Task<IEnumerable<Trip>> GetByTouristIdAsync(Guid touristId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Trip>> GetByStatusAsync(TripStatus status, CancellationToken cancellationToken = default);
    Task<IEnumerable<Trip>> GetUpcomingTripsAsync(Guid touristId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Trip>> GetActiveTripsAsync(Guid touristId, CancellationToken cancellationToken = default);
}
