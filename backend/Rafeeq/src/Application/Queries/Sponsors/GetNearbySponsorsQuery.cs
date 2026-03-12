using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors;

public record GetNearbySponsorsQuery(
    double Latitude,
    double Longitude,
    int RadiusKm = 3,
    SponsorFilters? Filters = null,
    PagingParameters? Paging = null) : IQuery<List<NearbySponsorDto>>;

internal class GetNearbySponsorsQueryHandler(
    ISponsorQueryService queryService) : IQueryHandler<GetNearbySponsorsQuery, List<NearbySponsorDto>>
{
    public async Task<Result<List<NearbySponsorDto>>> HandleAsync(GetNearbySponsorsQuery query, CancellationToken cancellationToken)
    {
        var sponsorListDtos = await queryService.GetNearbyAsync(
            query.Latitude,
            query.Longitude,
            query.RadiusKm,
            query.Filters,
            cancellationToken);

            return Result.Success(sponsorListDtos);
    }
}