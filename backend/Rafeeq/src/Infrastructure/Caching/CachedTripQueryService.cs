using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Trips;
using Microsoft.Extensions.Caching.Memory;

namespace Infrastructure.Caching;

internal class CachedTripQueryService(ITripQueryService inner, IMemoryCache cache)
    : BaseCache("trip", cache), ITripQueryService
{
    public async Task<TripDetailDto?> GetByIdAsync(Guid id, Guid touristId, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:touristId:{touristId}:detail:id:{id}";
        return await GetOrCreateNullableAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByIdAsync(id, touristId, cancellationToken));
    }

    public async Task<PagedResult<TripListDto>> GetByTouristIdAsync(Guid touristId, PagingParameters paging, CancellationToken cancellationToken = default)
    {
        var key = $"{Prefix}:touristId:{touristId}:list";
        return await GetOrCreateAsync(
            key,
            LongTtl30_Minutes,
            () => inner.GetByTouristIdAsync(touristId, paging, cancellationToken));
    }
}