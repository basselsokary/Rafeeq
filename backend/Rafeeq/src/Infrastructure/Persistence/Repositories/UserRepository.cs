using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.TouristAggregate;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal class TouristRepository(ApplicationDbContext context)
    : BaseRepository<Tourist>(context), ITouristRepository
{
    public async Task<Tourist?> GetWithFavouritesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Favourites)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

}
