using Domain.Entities.TripAggregate;
using Domain.Repositories;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

internal sealed class TripRepository(ApplicationDbContext context)
    : BaseRepository<Trip>(context), ITripRepository
{
    public async Task<Trip?> GetWithNotesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(t => t.Notes)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }

    public async Task<Trip?> GetWithSitesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await DbSet.Include(t => t.Sites)
            .FirstOrDefaultAsync(t => t.Id == id, cancellationToken);
    }
}