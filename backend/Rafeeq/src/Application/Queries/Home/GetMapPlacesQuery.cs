using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;

namespace Application.Queries.Home;

public record GetMapPlacesQuery(
    double Latitude,
    double Longitude,
    int RadiusKm = 20) : IQuery<List<MapPlaceMarkerDto>>;

internal sealed class GetMapPlacesQueryHandler(
    ISiteQueryService siteQueryService,
    ISponsorQueryService sponsorQueryService,
    IUserContext userContext) : IQueryHandler<GetMapPlacesQuery, List<MapPlaceMarkerDto>>
{
    public async Task<Result<List<MapPlaceMarkerDto>>> HandleAsync(GetMapPlacesQuery query, CancellationToken cancellationToken)
    {
        var siteMarkers = await siteQueryService.GetNearbyMarkerAsync(
            query.Latitude,
            query.Longitude,
            query.RadiusKm,
            language: userContext.Language,
            cancellationToken: cancellationToken);

        var sitePlaceMarkers = siteMarkers.Select(site => new MapPlaceMarkerDto(
            site.Id,
            site.Name,
            site.Location,
            "Site",
            site.PrimaryImageUrl
        )).ToList();
        
        var sponsorMarkers = await sponsorQueryService.GetNearbyMarkerAsync(
            query.Latitude,
            query.Longitude,
            query.RadiusKm,
            count: 5,
            language: userContext.Language,
            cancellationToken: cancellationToken);

        var sponsorPlaceMarkers = sponsorMarkers.Select(sponsor => new MapPlaceMarkerDto(
            sponsor.Id,
            sponsor.Name,
            sponsor.Location,
            "Sponsor",
            sponsor.PrimaryImageUrl
        )).ToList();

        var allPlaceMarkers = sitePlaceMarkers.Concat(sponsorPlaceMarkers).ToList();

        return Result.Success(allPlaceMarkers);
    }
}
