using Microsoft.EntityFrameworkCore;
using Domain.Entities.CityAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal sealed class CityRepository(ApplicationDbContext context)
    : BaseRepository<City>(context), ICityRepository
{
    public async Task<IEnumerable<City>> GetAllWithLocalizedContentsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.LocalizedContents)
            .ToListAsync(cancellationToken);
    }

    public async Task<City?> GetWithLocalizedContentsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(c => c.LocalizedContents)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
}
