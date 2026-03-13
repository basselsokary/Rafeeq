using Domain.Entities.AttractionAggregate;
using Domain.Enums;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal class AttractionRepository(ApplicationDbContext context)
    : BaseRepository<Attraction>(context), IAttractionRepository
{
    public Task<IEnumerable<Attraction>> GetByTypeAsync(AttractionType type, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<Attraction>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
