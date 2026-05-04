using Domain.Entities.TripAggregate;

namespace Domain.Repositories;

public interface ITripRepository : IBaseRepository<Trip>
{
    Task<Trip?> GetWithSitesAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Trip?> GetWithNotesAsync(Guid id, CancellationToken cancellationToken = default); 
}
