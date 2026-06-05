using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedTouristQueryService(ITouristQueryService inner, IMemoryCache cache)
    : BaseCache("tourist", cache), ITouristQueryService
{
    public async Task<TouristProfileDto?> GetProfileByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:profile:id:{id}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetProfileByIdAsync(id, cancellationToken));
    }

    public async Task<PagedResult<FavoriteSiteDto>> GetFavoriteSitesAsync(Guid touristId, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:favorites:sites:{touristId}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetFavoriteSitesAsync(touristId, paging, language, cancellationToken));
    }

    public async Task<bool> HasFavoritedSiteAsync(Guid touristId, Guid siteId, CancellationToken cancellationToken = default)
    {
        return await inner.HasFavoritedSiteAsync(touristId, siteId, cancellationToken);
    }

    public async Task<List<Guid>> GetFavoriteSiteIdsAsync(Guid touristId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:favorites:ids:{touristId}";
        return await GetOrCreateAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.GetFavoriteSiteIdsAsync(touristId, cancellationToken));
    }

    public async Task<PagedResult<VisitedSiteDto>> GetVisitedSitesAsync(Guid id, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:visited:sites:{id}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.GetVisitedSitesAsync(id, paging, language, cancellationToken));
    }
}