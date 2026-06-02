using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;

namespace Application.Queries.Home;

public record GetMapPlacesQuery(
    double Latitude,
    double Longitude,
    int RadiusKm = 30,
    int Count = 10) : IQuery<List<MapPlaceMarkerDto>>;

internal sealed class GetMapPlacesQueryHandler(
    IMapQueryService mapQueryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetMapPlacesQuery, List<MapPlaceMarkerDto>>
{
    public async Task<Result<List<MapPlaceMarkerDto>>> HandleAsync(GetMapPlacesQuery query, CancellationToken cancellationToken)
    {
        var places = await mapQueryService.GetNearbyMarkersAsync(
            latitude: query.Latitude,
            longitude: query.Longitude,
            radiusKm: query.RadiusKm,
            count: query.Count,
            userContext.Language,
            cancellationToken);

        return Result.Success(places.Select(p => p with
        { 
            TypeDisplay = enumLocalizer.Localize(p.Type)
        }).ToList());
    }
}
