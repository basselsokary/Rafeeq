using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Infrastructure.Caching;

internal class CachedSponsorQueryService(ISponsorQueryService inner, IMemoryCache cache)
    : BaseCache("sponsor", cache), ISponsorQueryService
{
    public async Task<SponsorDetailDto?> GetByIdAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:detail:{id}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByIdAsync(id, language, cancellationToken));
    }

    public async Task<AdminSponsorDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:admin:{id}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetByIdForAdminAsync(id, cancellationToken));
    }

    public async Task<PagedResult<SponsorListDto>> GetAsync(SponsorFilters filters, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:{FormatFilters(filters)}:{FormatPaging(paging)}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetAsync(filters, paging, language, cancellationToken));
    }

    public async Task<List<NearbySponsorDto>> GetNearbyAsync(
        double latitude,
        double longitude,
        SponsorFilters filters,
        double radiusKm = 3,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var key =
            $"{Prefix}:list:nearby:{FormatCoordinate(latitude)}:{FormatCoordinate(longitude)}:{FormatCoordinate(radiusKm)}:{count}:{FormatFilters(filters)}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetNearbyAsync(latitude, longitude, filters, radiusKm, count, language, cancellationToken));
    }

    public async Task<PagedResult<SponsorListDto>> SearchAsync(
        string searchTerm,
        SponsorFilters filters,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var normalizedSearch = searchTerm.Trim().ToLowerInvariant();
        var key = $"{Prefix}:list:search:{normalizedSearch}:{FormatFilters(filters)}:{FormatPaging(paging)}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.SearchAsync(searchTerm, filters, paging, language, cancellationToken));
    }

    public async Task<List<SponsorOfferDto>> GetOffersAsync(Guid sponsorId, bool activeOnly = true, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:offers:list:{sponsorId}:active={activeOnly}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetOffersAsync(sponsorId, activeOnly, language, cancellationToken));
    }

    public async Task<SponsorOfferDto?> GetOfferByIdAsync(Guid offerId, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:offer:{offerId}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetOfferByIdAsync(offerId, language, cancellationToken));
    }

    public async Task<List<ImageDto>> GetImagesAsync(
        Guid sponsorId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:images:{sponsorId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetImagesAsync(sponsorId, cancellationToken));
    }

    public async Task<ImageDto?> GetImageByIdAsync(
        Guid sponsorId,
        Guid imageId,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:image:{sponsorId}:{imageId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetImageByIdAsync(sponsorId, imageId, cancellationToken));
    }

    public async Task<PagedResult<SponsorOfferListDto>> GetAllOffersAsync(
        SponsorFilters filters,
        PagingParameters paging,
        bool activeOnly = true,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:offers:list:active={activeOnly}:{FormatFilters(filters)}:{FormatPaging(paging)}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetAllOffersAsync(filters, paging, activeOnly, language, cancellationToken));
    }

    public async Task<List<SponsorOfferSummaryDto>> GetActiveOffersAsync(int count = 10, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:offers:list:active:{count}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetActiveOffersAsync(count, language, cancellationToken));
    }

    public async Task<AdminOfferLocalizedContentDto?> GetOfferLocalizedContentByIdAsync(Guid offerId, Guid contentId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:offer:content:{offerId}:{contentId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetOfferLocalizedContentByIdAsync(offerId, contentId, cancellationToken));
    }

    public async Task<List<AdminOfferLocalizedContentDto>> GetOfferLocalizedContentsAsync(Guid offerId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:offer:contents:{offerId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetOfferLocalizedContentsAsync(offerId, cancellationToken));
    }

    public async Task<List<AdminSponsorLocalizedContentDto>> GetLocalizedContentsAsync(Guid sponsorId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:contents:{sponsorId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetLocalizedContentsAsync(sponsorId, cancellationToken));
    }

    public async Task<AdminSponsorLocalizedContentDto?> GetLocalizedContentByIdAsync(Guid sponsorId, Guid contentId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:content:{sponsorId}:{contentId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetLocalizedContentByIdAsync(sponsorId, contentId, cancellationToken));
    }

    public async Task<List<SponsorMapMarkerDto>> GetNearbyMarkerAsync(
        double latitude,
        double longitude,
        int radiusKm = 20,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var key =
            $"{Prefix}:list:nearby:markers:{FormatCoordinate(latitude)}:{FormatCoordinate(longitude)}:{FormatCoordinate(radiusKm)}:{count}:{language}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetNearbyMarkerAsync(latitude, longitude, radiusKm, count, language, cancellationToken));
    }

    public async Task<AdminSponsorDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:dashboard:admin";
        return await GetOrCreateAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetDashboardAsync(cancellationToken));
    }

    public async Task<bool> AnyAsync(Guid sponsorId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:any:{sponsorId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.AnyAsync(sponsorId, cancellationToken));
    }

    private static string FormatFilters(SponsorFilters filters)
    {
        var type = filters.Type?.ToString() ?? "all";
        var tier = filters.Tier?.ToString() ?? "all";
        var activeOnly = filters.ActiveOnly?.ToString() ?? "all";
        return $"type={type}:tier={tier}:active={activeOnly}";
    }

    private static string FormatCoordinate(double value)
    {
        value = Math.Round(value, 4);
        return value.ToString("F4", CultureInfo.InvariantCulture);
    }
}