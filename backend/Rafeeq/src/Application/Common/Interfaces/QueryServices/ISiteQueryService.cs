using Application.DTOs.Sites;
using Application.DTOs.Common;
using Application.DTOs.Admins;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface ISiteQueryService
{
    Task<SiteDetailDto?> GetByIdAsync(
        Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    Task<SiteDetailDto?> GetByNameAsync(
        string name, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);
    Task<AdminSiteDetailDto?> GetByIdForAdminAsync(
        Guid id, CancellationToken cancellationToken = default);
        
    Task<List<SiteListDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SiteFilters filters,
        int radiusKm = 40,
        int count = 20,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SiteListDto>> GetByStatusAsync(
        SiteStatus status,
        SiteType type,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    /// for map view
    Task<List<SiteMapMarkerDto>> GetWithinBoundsAsync(
        BoundingBox bounds,
        SiteFilters filters,
        int count = 20,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<List<SiteListDto>> GetByNamesAsync(
        List<string> names,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<List<SiteSummaryDto>> GetMustVisitAsync(
        int count = 10, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);

    Task<List<SiteSummaryDto>> GetHiddenGemsAsync(
        int count = 10, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default);

    Task<List<SiteSummaryDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        double radiusKm = 40,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<List<SiteListDto>> GetFeaturedAsync(
        int count = 10,
        Guid? city = null,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<PagedResult<SiteListDto>> SearchAsync(
        string searchTerm,
        SiteFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<List<SiteLookupDto>> SearchAsync(
        string searchTerm,
        int count = 10,
        CancellationToken cancellationToken = default);

    Task<PagedResult<SiteListDto>> GetAsync(
        SiteFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
    
    Task<List<SiteListDto>> GetSimilarAsync(
        Guid siteId,
        int count = 5,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<List<AdminSiteLocalizedContentDto>> GetLocalizedContentsAsync(
        Guid siteId, CancellationToken cancellationToken = default);

    Task<AdminSiteLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid siteId, Guid contentId, CancellationToken cancellationToken = default);

    Task<List<AdminSiteFacilityDto>> GetFacilitiesAsync(
        Guid siteId, CancellationToken cancellationToken = default);
    
    Task<List<AdminSiteNearestTransportationDto>> GetNearestTransportationAsync(
        Guid siteId, CancellationToken cancellationToken = default);

    Task<AdminSiteNearestTransportationDto?> GetNearestTransportationByIdAsync(
        Guid siteId, Guid transportationId, CancellationToken cancellationToken = default);

    Task<AdminSiteNearestTransportationLocalizedContentDto?> GetNearestTransportationLocalizedContentByIdAsync(
        Guid siteId,
        Guid transportationId,
        Guid contentId,
        CancellationToken cancellationToken = default);
    
    Task<List<ImageDto>> GetImagesAsync(
        Guid siteId, CancellationToken cancellationToken = default);

    Task<ImageDto?> GetImageByIdAsync(
        Guid siteId, Guid imageId, CancellationToken cancellationToken = default);
    
    Task<List<SiteMapMarkerDto>> GetNearbyMarkerAsync(
        double latitude,
        double longitude,
        int radiusKm = 40,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);

    Task<List<AdminSiteOpeningHourDto>> GetOpeningHoursAsync(Guid siteId, CancellationToken cancellationToken);

    Task<AdminSiteDashboardDto> GetDashboardAsync(CancellationToken cancellationToken);
    Task<bool> AnyAsync(Guid siteId, CancellationToken cancellationToken);
}
