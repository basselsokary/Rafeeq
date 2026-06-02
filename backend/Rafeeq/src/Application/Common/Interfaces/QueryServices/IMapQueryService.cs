using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Common.Interfaces.QueryServices;

public interface IMapQueryService
{
    Task<List<MapPlaceMarkerDto>> GetNearbyMarkersAsync(
        double latitude,
        double longitude,
        int radiusKm = 40,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default);
}