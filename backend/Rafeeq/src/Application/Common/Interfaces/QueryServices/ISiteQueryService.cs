using Application.DTOs.Sites;
using Application.DTOs.Common;

namespace Application.Common.Interfaces.QueryServices;

public interface ISiteQueryService
{
    Task<SiteDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    Task<List<SiteListDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        int radiusKm = 5,
        SiteFilters? filters = null,
        CancellationToken cancellationToken = default);
    
    /// for map view
    Task<List<SiteMapMarkerDto>> GetWithinBoundsAsync(
        BoundingBox bounds,
        SiteFilters? filters = null,
        int count = 20,
        CancellationToken cancellationToken = default);

    Task<List<SiteListDto>> GetFeaturedAsync(
        int count = 10,
        Guid? city = null,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<SiteListDto>> SearchAsync(
        string searchTerm,
        SiteFilters? filters = null,
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SiteListDto>> GetAllAsync(
        SiteFilters? filters = null,
        PagingParameters? paging = null,
        CancellationToken cancellationToken = default);

    Task<List<LocalizedContentDto>> GetLocalizedContentsAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get personalized recommendations for a user
    /// </summary>
    // Task<List<SiteListDto>> GetRecommendationsAsync(
    //     Guid userId,
    //     double? userLatitude = null,
    //     double? userLongitude = null,
    //     int count = 10,
    //     CancellationToken cancellationToken = default);

    Task<List<SiteListDto>> GetSimilarAsync(
        Guid siteId,
        int count = 5,
        CancellationToken cancellationToken = default);
        
    // Task<Dictionary<string, int>> GetCountByTypeAsync(CancellationToken cancellationToken = default);
}
