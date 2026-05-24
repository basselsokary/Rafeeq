using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface ITouristQueryService
{
    Task<TouristProfileDto?> GetProfileByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(
        Guid touristId,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<bool> HasFavoritedSiteAsync(
        Guid touristId,
        Guid siteId,
        CancellationToken cancellationToken = default);
    
    Task<List<Guid>> GetFavoriteSiteIdsAsync(
        Guid touristId,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<VisitedSiteDto>> GetVisitedSitesAsync(
        Guid id,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
}