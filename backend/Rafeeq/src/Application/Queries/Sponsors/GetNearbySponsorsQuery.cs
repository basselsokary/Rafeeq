using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sponsors;

namespace Application.Queries.Sponsors;

public sealed record GetNearbySponsorsQuery(
    double Latitude,
    double Longitude,
    SponsorFilters Filters,
    int Count = 10,
    int RadiusKm = 3) : IQuery<List<NearbySponsorDto>>;

internal sealed class GetNearbySponsorsQueryHandler(
    ISponsorQueryService queryService,
    IEnumLocalizer enumLocalizer,
    IUserContext userContext) : IQueryHandler<GetNearbySponsorsQuery, List<NearbySponsorDto>>
{
    public async Task<Result<List<NearbySponsorDto>>> HandleAsync(GetNearbySponsorsQuery query, CancellationToken cancellationToken)
    {
        var sponsorListDtos = await queryService.GetNearbyAsync(
            query.Latitude,
            query.Longitude,
            query.Filters,
            query.RadiusKm,
            query.Count,
            userContext.Language,
            cancellationToken);

        var localizedSponsorListDtos = sponsorListDtos.Select(sponsor => sponsor with
        {
            TypeDisplay = enumLocalizer.Localize(sponsor.Type)
        }).ToList();

        return Result.Success(localizedSponsorListDtos);
    }
}