using Application.DTOs.Sites;
using Application.DTOs.Common;
using Application.DTOs.Admins;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface ISiteQueryService
{
    Task<SiteDetailDto?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);
    Task<SiteAdminDetailDto?> GetByIdForAdminAsync(
        Guid id,
        CancellationToken cancellationToken = default);
        
    Task<List<SiteListDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SiteFilters filters,
        int radiusKm = 5,
        CancellationToken cancellationToken = default);

    Task<PagedResult<AdminSiteListDto>> GetByStatusAsync(
        SiteStatus status,
        PagingParameters paging,
        CancellationToken cancellationToken = default);
    
    /// for map view
    Task<List<SiteMapMarkerDto>> GetWithinBoundsAsync(
        BoundingBox bounds,
        SiteFilters filters,
        int count = 20,
        CancellationToken cancellationToken = default);

    Task<List<SiteListDto>> GetFeaturedAsync(
        int count = 10,
        Guid? city = null,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<SiteListDto>> SearchAsync(
        string searchTerm,
        SiteFilters filters,
        PagingParameters paging,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SiteListDto>> GetAsync(
        SiteFilters filters,
        PagingParameters paging,
        CancellationToken cancellationToken = default);

    Task<List<DTOs.Sites.LocalizedContentDto>> GetLocalizedContentsAsync(
        Guid siteId,
        CancellationToken cancellationToken = default);

    Task<List<SiteListDto>> GetSimilarAsync(
        Guid siteId,
        int count = 5,
        CancellationToken cancellationToken = default);
}
