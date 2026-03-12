using Application.DTOs.Common;
using Application.DTOs.Users;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IUserQueryService
{
    Task<UserProfileDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<UserProfileDto?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<UserListDto>> GetAllAsync(
        string? searchTerm, // by name or email
        UserRole? role = null,
        UserStatus status = UserStatus.Active,
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(
        Guid userId,
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);
    
    Task<bool> HasFavoritedSiteAsync(
        Guid userId,
        Guid siteId,
        CancellationToken cancellationToken = default);
    
    Task<List<Guid>> GetFavoriteSiteIdsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}