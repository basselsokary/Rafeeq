using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Tourists;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedTouristQueryService(ITouristQueryService inner, IMemoryCache cache)
    : BaseCache("tourist", cache), ITouristQueryService
{
    public async Task<TouristProfileDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:profile:id:{id}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByIdAsync(id, cancellationToken));
    }

    public async Task<TouristAdminDetailDto?> GetByIdForAdminAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:admin:id:{id}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetByIdForAdminAsync(id, cancellationToken));
    }

    public async Task<TouristProfileDto?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var key = $"{Prefix}:profile:email:{normalizedEmail}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByEmailAsync(email, cancellationToken));
    }

    public async Task<TouristAdminDetailDto?> GetByEmailForAdminAsync(string email, CancellationToken cancellationToken = default)
    {
        var normalizedEmail = NormalizeEmail(email);
        var key = $"{Prefix}:admin:email:{normalizedEmail}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetByEmailForAdminAsync(email, cancellationToken));
    }

    public async Task<PagedResult<TouristListDto>> GetAllAsync(PagingParameters paging, string? searchTerm = null, UserStatus status = UserStatus.Active, bool? emailVerified = null, CancellationToken cancellationToken = default)
    {
        var normalizedSearch = (searchTerm ?? string.Empty).Trim().ToLowerInvariant();
        var key = $"{Prefix}:list:search={normalizedSearch}:status={status}:email-verified={emailVerified?.ToString() ?? "all"}:{FormatPaging(paging)}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetAllAsync(paging, searchTerm, status, emailVerified, cancellationToken));
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
    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }

    public Task<PagedResult<VisitedSiteDto>> GetVisitedSitesAsync(Guid id, PagingParameters paging, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:visited:sites:{id}:{FormatPaging(paging)}";
        return GetOrCreateAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.GetVisitedSitesAsync(id, paging, language, cancellationToken));
    }
}