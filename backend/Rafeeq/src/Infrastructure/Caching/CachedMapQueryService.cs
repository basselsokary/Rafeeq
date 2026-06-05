using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Domain.Enums;
using Microsoft.Extensions.Caching.Memory;
using System.Globalization;

namespace Infrastructure.Caching;

internal class CachedMapQueryService(IMapQueryService inner, IMemoryCache cache)
    : BaseCache("map", cache), IMapQueryService
{
    public async Task<List<MapPlaceMarkerDto>> GetNearbyMarkersAsync(double latitude, double longitude, int radiusKm = 40, int count = 10, LanguageCode language = LanguageCode.English, CancellationToken cancellationToken = default)
    {
        var key =
            $"{Prefix}:nearby-markers:{FormatCoordinate(latitude)}:{FormatCoordinate(longitude)}:{radiusKm}:{count}:{language}";
        return await GetOrCreateAsync(
            key,
            MediumTtl10_Minutes,
            () => inner.GetNearbyMarkersAsync(latitude, longitude, radiusKm, count, language, cancellationToken));
    }

    private static string FormatCoordinate(double value)
    {
        value = Math.Round(value, 4);
        return value.ToString("F4", CultureInfo.InvariantCulture);
    }
}
