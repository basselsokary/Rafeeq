using Domain.Enums;

namespace Application.Common.Interfaces.Services;

public interface IAdminService
{
    Task<Result> AddModeratorAsync(
        string userName,
        string firstName,
        string lastName,
        string fullName,
        string email,
        string password,
        CancellationToken cancellationToken);

    Task<Result> AssignRoleAsync(Guid userId, UserRole role);
    Task<Result> RemoveRoleAsync(Guid userId, UserRole role);
    Task<Result> DeleteUserAsync(string email);
}