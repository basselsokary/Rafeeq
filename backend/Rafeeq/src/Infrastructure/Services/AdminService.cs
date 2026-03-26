using Application.Common.Interfaces.Services;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Shared.Models;

namespace Infrastructure.Services;

internal class AdminService(
    ApplicationDbContext context) : IAdminService
{
    public Task<Result> AssignRoleAsync(Guid userId, UserRole role)
    {
        throw new NotImplementedException();
    }

    public Task<Result> RemoveRoleAsync(Guid userId, UserRole role)
    {
        throw new NotImplementedException();
    }
}
