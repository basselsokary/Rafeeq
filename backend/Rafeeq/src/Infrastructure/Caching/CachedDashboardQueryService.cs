using Application.Common.Interfaces.QueryServices;
using Application.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedDashboardQueryService(IDashboardQueryService inner, IMemoryCache cache)
    : BaseCache("dashboard", cache), IDashboardQueryService
{
    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var key = $"{Prefix}:stats";
        return await GetOrCreateAsync(
            key,
            ShortTtl5_Minutes,
            inner.GetDashboardStatsAsync);
    }
}
