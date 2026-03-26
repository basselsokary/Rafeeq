using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IUserQueryService
{
    Task<TouristProfileDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<AdminUserDetailDto?> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TouristProfileDto?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
    
    Task<AdminUserDetailDto?> GetByEmailForAdminAsync(
        string email,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<TouristListDto>> GetAllAsync(
        PagingParameters paging,
        string? searchTerm = null, // by name or email
        UserRole? role = null,
        UserStatus status = UserStatus.Active,
        bool? emailVerified = null,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(
        Guid touristId,
        PagingParameters paging,
        CancellationToken cancellationToken = default);
    
    Task<bool> HasFavoritedSiteAsync(
        Guid touristId,
        Guid siteId,
        CancellationToken cancellationToken = default);
    
    Task<List<Guid>> GetFavoriteSiteIdsAsync(
        Guid touristId,
        CancellationToken cancellationToken = default);
}