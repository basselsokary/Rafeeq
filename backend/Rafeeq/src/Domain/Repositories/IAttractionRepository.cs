using Domain.Entities.AttractionAggregate;
using Domain.Enums;

namespace Domain.Repositories;

public interface IAttractionRepository : IBaseRepository<Attraction>
{
    Task<IEnumerable<Attraction>> GetByTypeAsync(AttractionType type, CancellationToken cancellationToken = default);
    Task<IEnumerable<Attraction>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
}
