using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.Sites;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Infrastructure.Caching;

internal class CachedSiteQueryService(ISiteQueryService inner, IMemoryCache cache)
    : BaseCache("site", cache), ISiteQueryService
{
    public async Task<PagedResult<SiteListDto>> GetAsync(SiteFilters filters, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:{FormatFilters(filters)}:{FormatPaging(paging)}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetAsync(filters, paging, language, cancellationToken));
    }

    public async Task<SiteDetailDto?> GetByIdAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:detail:{id}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByIdAsync(id, language, cancellationToken));
    }

    public async Task<SiteDetailDto?> GetByNameAsync(string name, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
            return null;

        var normalized = name.Trim().ToLowerInvariant();
        var key = $"{Prefix}:detail:name:{normalized}:{language}";

        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByNameAsync(name, language, cancellationToken));
    }

    public async Task<AdminSiteDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:admin:{id}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl2_Minutes,
            () => inner.GetByIdForAdminAsync(id, cancellationToken));
    }

    public async Task<PagedResult<SiteListDto>> GetByStatusAsync(SiteStatus status, SiteType type, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:status:{status}:{type}:{FormatPaging(paging)}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetByStatusAsync(status, type, paging, language, cancellationToken));
    }

    public async Task<List<SiteListDto>> GetByNamesAsync(List<string> names, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        return await inner.GetByNamesAsync(names, language, cancellationToken);
    }

    public async Task<List<SiteListDto>> GetFeaturedAsync(int count = 10, Guid? city = null, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:featured:{count}:{city?.ToString() ?? "none"}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetFeaturedAsync(count, city, language, cancellationToken));
    }

    public async Task<List<SiteSummaryDto>> GetHiddenGemsAsync(int count = 10, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:hidden-gems:{count}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetHiddenGemsAsync(count, language, cancellationToken));
    }

    public async Task<List<SiteSummaryDto>> GetMustVisitAsync(int count = 10, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:must-visit:{count}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetMustVisitAsync(count, language, cancellationToken));
    }

    public async Task<List<SiteListDto>> GetNearbyAsync(double latitude, double longitude, SiteFilters filters, int radiusKm = 5, int count = 20, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key =
            $"{Prefix}:list:nearby:{FormatCoordinate(latitude)}:{FormatCoordinate(longitude)}:{radiusKm}:{count}:{FormatFilters(filters)}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetNearbyAsync(latitude, longitude, filters, radiusKm, count, language, cancellationToken));
    }

    public async Task<List<SiteSummaryDto>> GetNearbyAsync(double latitude, double longitude, double radiusKm = 50, int count = 10, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key =
            $"{Prefix}:list:nearby:summary:{FormatCoordinate(latitude)}:{FormatCoordinate(longitude)}:{FormatCoordinate(radiusKm)}:{count}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetNearbyAsync(latitude, longitude, radiusKm, count, language, cancellationToken));
    }

    public async Task<List<SiteListDto>> GetSimilarAsync(Guid siteId, int count = 5, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:similar:{siteId}:{count}:{language}";
        return await GetOrCreateAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetSimilarAsync(siteId, count, language, cancellationToken));
    }

    public async Task<List<SiteMapMarkerDto>> GetWithinBoundsAsync(BoundingBox bounds, SiteFilters filters, int count = 20, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:bounds:{FormatBounds(bounds)}:{count}:{FormatFilters(filters)}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetWithinBoundsAsync(bounds, filters, count, language, cancellationToken));
    }

    public async Task<PagedResult<SiteListDto>> SearchAsync(string searchTerm, SiteFilters filters, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var normalizedSearch = searchTerm.Trim().ToLowerInvariant();
        var key = $"{Prefix}:list:search:{normalizedSearch}:{FormatFilters(filters)}:{FormatPaging(paging)}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.SearchAsync(searchTerm, filters, paging, language, cancellationToken));
    }

    public async Task<List<SiteLookupDto>> SearchAsync(string searchTerm, int count = 10, CancellationToken cancellationToken = default)
    {
        var normalizedSearch = searchTerm.Trim().ToLowerInvariant();
        var key = $"{Prefix}:list:search:{normalizedSearch}:{count}";
        return await GetOrCreateAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.SearchAsync(searchTerm, count, cancellationToken));
    }

    public async Task<List<AdminSiteLocalizedContentDto>> GetLocalizedContentsAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:localized-contents:{siteId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetLocalizedContentsAsync(siteId, cancellationToken));
    }

    public async Task<AdminSiteLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid siteId,
        Guid contentId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:localized-content:{siteId}:{contentId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetLocalizedContentByIdAsync(siteId, contentId, cancellationToken));
    }

    public async Task<List<AdminSiteFacilityDto>> GetFacilitiesAsync(
        Guid siteId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:facilities:{siteId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetFacilitiesAsync(siteId, cancellationToken));
    }

    public async Task<List<AdminSiteNearestTransportationDto>> GetNearestTransportationAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:nearest-transportation:{siteId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetNearestTransportationAsync(siteId, cancellationToken));
    }

    public async Task<AdminSiteNearestTransportationDto?> GetNearestTransportationByIdAsync(
        Guid siteId,
        Guid transportationId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:nearest-transportation:{siteId}:{transportationId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetNearestTransportationByIdAsync(siteId, transportationId, cancellationToken));
    }

    public async Task<AdminSiteNearestTransportationLocalizedContentDto?> GetNearestTransportationLocalizedContentByIdAsync(
        Guid siteId,
        Guid transportationId,
        Guid contentId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:nearest-transportation-localized-content:{siteId}:{transportationId}:{contentId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetNearestTransportationLocalizedContentByIdAsync(siteId, transportationId, contentId, cancellationToken));
    }

    public Task<List<ImageDto>> GetImagesAsync(Guid siteId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:images:{siteId}";
        return GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetImagesAsync(siteId, cancellationToken));
    }

    public Task<ImageDto?> GetImageByIdAsync(
        Guid siteId,
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:image:{siteId}:{imageId}";
        return GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetImageByIdAsync(siteId, imageId, cancellationToken));
    }

    public async Task<List<SiteMapMarkerDto>> GetNearbyMarkerAsync(
        double latitude,
        double longitude,
        int radiusKm = 20,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var key =
            $"{Prefix}:list:nearby:markers:{FormatCoordinate(latitude)}:{FormatCoordinate(longitude)}:{radiusKm}:{count}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetNearbyMarkerAsync(latitude, longitude, radiusKm, count, language, cancellationToken));
    }

    public Task<List<AdminSiteOpeningHourDto>> GetOpeningHoursAsync(Guid siteId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:opening-hours:{siteId}";
        return GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetOpeningHoursAsync(siteId, cancellationToken));
    }

    public Task<AdminSiteDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:dashboard";
        return GetOrCreateAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetDashboardAsync(cancellationToken));
    }

    public Task<bool> AnyAsync(Guid siteId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:any:{siteId}";
        return GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.AnyAsync(siteId, cancellationToken));
    }

    private static string FormatFilters(SiteFilters filters)
    {
        var type = filters.Type?.ToString() ?? "all";
        var city = filters.City?.ToString() ?? "all";
        var isFree = filters.IsFree?.ToString() ?? "all";
        var minRating = filters.MinRating?.ToString(CultureInfo.InvariantCulture) ?? "all";
        var maxRating = filters.MaxRating?.ToString(CultureInfo.InvariantCulture) ?? "all";
        return $"type={type}:city={city}:free={isFree}:min={minRating}:max={maxRating}";
    }

    private static string FormatBounds(BoundingBox bounds)
    {
        return
            $"n={FormatCoordinate(bounds.NorthLatitude)}:s={FormatCoordinate(bounds.SouthLatitude)}:e={FormatCoordinate(bounds.EastLongitude)}:w={FormatCoordinate(bounds.WestLongitude)}";
    }

    private static string FormatCoordinate(double value)
    {
        value = Math.Round(value, 4);
        return value.ToString("F4", CultureInfo.InvariantCulture);
    }
}
