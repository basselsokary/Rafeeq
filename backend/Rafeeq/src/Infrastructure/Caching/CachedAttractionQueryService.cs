using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedAttractionQueryService(IAttractionQueryService inner, IMemoryCache cache)
    : BaseCache("attraction", cache), IAttractionQueryService
{
    public async Task<AttractionDetailDto?> GetByIdAsync(
        Guid id,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:detail:{id}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByIdAsync(id, language, cancellationToken));
    }

    public async Task<AttractionAdminDetailDto?> GetByIdForAdminAsync(
        Guid siteId,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:admin:{siteId}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetByIdForAdminAsync(siteId, language, cancellationToken));
    }

    public async Task<PagedResult<AttractionListDto>> GetBySiteIdAsync(
        Guid siteId,
        AttractionType? type,
        PagingParameters paging,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:site:{siteId}:{type}:{language}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetBySiteIdAsync(siteId, type, paging, language, cancellationToken));
    }

    public async Task<List<AttractionLocalizedContentDto>> GetLocalizedContentsAsync(Guid attractionId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:localized-contents:{attractionId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetLocalizedContentsAsync(attractionId, cancellationToken));
    }

    public async Task<AttractionLocalizedContentDto?> GetLocalizedContentByIdAsync(
        Guid attractionId,
        Guid contentId,
        CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:localized-content:{attractionId}:{contentId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetLocalizedContentByIdAsync(attractionId, contentId, cancellationToken));
    }

    public async Task<List<ImageDto>> GetImagesAsync(Guid attractionId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:images:{attractionId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetImagesAsync(attractionId, cancellationToken));
    }

    public async Task<ImageDto?> GetImageByIdAsync(Guid attractionId, Guid imageId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:image:{attractionId}:{imageId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetImageByIdAsync(attractionId, imageId, cancellationToken));
    }

    public Task<PagedResult<AttractionListDto>> GetAllAsync(string? searchTerm, AttractionType? type, PagingParameters paging, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:list:{searchTerm}:{type}:{FormatPaging(paging)}";
        return GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetAllAsync(searchTerm, type, paging, cancellationToken));
    }

    public Task<AdminAttractionDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:dashboard";
        return GetOrCreateAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetDashboardAsync(cancellationToken));
    }
}
