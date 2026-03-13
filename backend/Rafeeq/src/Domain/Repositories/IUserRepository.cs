using Domain.Entities.UserAggregate;

namespace Domain.Repositories;

public interface IUserRepository : IBaseRepository<User>
{
    Task<User?> GetWithFavouritesAsync(Guid id, CancellationToken cancellationToken = default);
}
