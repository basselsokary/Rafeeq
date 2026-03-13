using Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Domain.Entities.UserAggregate;
using Infrastructure.Persistence.ApplicationContext;

namespace Infrastructure.Persistence.Repositories;

internal class UserRepository(ApplicationDbContext context)
    : BaseRepository<User>(context), IUserRepository
{
    public async Task<User?> GetWithFavouritesAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(u => u.Favourites)
            .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
    }

}
