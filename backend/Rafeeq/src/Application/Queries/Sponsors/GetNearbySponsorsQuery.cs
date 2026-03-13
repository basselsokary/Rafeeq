using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors;

public record GetNearbySponsorsQuery(
    double Latitude,
    double Longitude,
    SponsorFilters Filters,
    int Count = 10,
    int RadiusKm = 3) : IQuery<List<NearbySponsorDto>>;

internal class GetNearbySponsorsQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetNearbySponsorsQuery, List<NearbySponsorDto>>
{
    public async Task<Result<List<NearbySponsorDto>>> HandleAsync(GetNearbySponsorsQuery query, CancellationToken cancellationToken)
    {
        var sponsorListDtos = await queryService.GetNearbyAsync(
            query.Latitude,
            query.Longitude,
            query.Filters,
            query.RadiusKm,
            query.Count,
            cancellationToken);

            return Result.Success(sponsorListDtos);
    }
}