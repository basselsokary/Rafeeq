using Microsoft.EntityFrameworkCore;
using Application.DTOs.Users;
using Application.Common.Interfaces.QueryServices;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;

namespace Infrastructure.Persistence.QueryServices;

internal class UserQueryService(ApplicationDbContext context) : IUserQueryService
{
    public Task<PagedResult<UserListDto>> GetAllAsync(string? searchTerm, UserRole? role = null, UserStatus status = UserStatus.Active, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<UserProfileDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<List<Guid>> GetFavoriteSiteIdsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(Guid userId, PagingParameters? paging = null, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task<bool> HasFavoritedSiteAsync(Guid userId, Guid siteId, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

}
