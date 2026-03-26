using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface ITouristQueryService
{
    Task<TouristProfileDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<TouristAdminDetailDto?> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<TouristProfileDto?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default);
    
    Task<TouristAdminDetailDto?> GetByEmailForAdminAsync(
        string email,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<TouristListDto>> GetAllAsync(
        PagingParameters paging,
        string? searchTerm = null, // by name or email
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