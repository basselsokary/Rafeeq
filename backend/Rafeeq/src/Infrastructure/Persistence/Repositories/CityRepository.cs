using Microsoft.EntityFrameworkCore;
using Domain.Entities.CityAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal class CityRepository(ApplicationDbContext context)
    : BaseRepository<City>(context), ICityRepository
{
    public Task<City?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbSet
            .Include(c => c.LocalizedContents)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
