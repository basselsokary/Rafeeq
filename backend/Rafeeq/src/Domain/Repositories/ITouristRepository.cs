using Domain.Enums;
using Domain.Entities.TouristAggregate;

namespace Domain.Repositories;

public interface ITouristRepository : IBaseRepository<User>
{
    // Task<Tourist?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    // Task<IEnumerable<Tourist>> GetByRoleAsync(UserRole role, CancellationToken cancellationToken = default);
    Task<IEnumerable<User>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default);
    // Task<bool> EmailExistsAsync(string email, CancellationToken cancellationToken = default);
}
