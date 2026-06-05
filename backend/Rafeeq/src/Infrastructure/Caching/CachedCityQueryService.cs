using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Application.DTOs.Cities;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedCityQueryService(ICityQueryService inner, IMemoryCache cache)
    : BaseCache("city", cache), ICityQueryService
{
    public async Task<CityDetailDto?> GetByIdAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:detail:{id}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByIdAsync(id, language, cancellationToken));
    }

    public async Task<CityAdminDetailDto?> GetByIdForAdminAsync(Guid id, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:admin:{id}:{language}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetByIdForAdminAsync(id, language, cancellationToken));
    }

    public async Task<List<CitySummaryDto>> GetSummariesAsync(LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:summaries:{language}";
        return await GetOrCreateAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetSummariesAsync(language, cancellationToken));
    }

    public async Task<List<CityListDto>> GetAsync(LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:list:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl20_Minutes,
            () => inner.GetAsync(language, cancellationToken));
    }

    public async Task<List<CityLocalizedContentDto>> GetLocalizedContentsAsync(Guid cityId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:localized-contents:{cityId}";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetLocalizedContentsAsync(cityId, cancellationToken));
    }

    public async Task<CityLocalizedContentDto?> GetLocalizedContentByIdAsync(Guid cityId, Guid contentId, CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:localized-content:{cityId}:{contentId}";
        return await GetOrCreateNullableAsync(
            key,
            ShortTtl5_Minutes,
            () => inner.GetLocalizedContentByIdAsync(cityId, contentId, cancellationToken));
    }

    public async Task<AdminCityDashboardDto> GetDashboardAsync(CancellationToken cancellationToken)
    {
        var key = $"{Prefix}:dashboard";
        return await GetOrCreateAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetDashboardAsync(cancellationToken));
    }
}