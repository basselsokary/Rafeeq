using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Trips;

namespace Application.Queries.Trips;

public sealed record GetTouristTripsQuery(PagingParameters Paging) : IQuery<PagedResult<TripListDto>>;

internal sealed class GetTouristTripsQueryHandler(
    ITripQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetTouristTripsQuery, PagedResult<TripListDto>>
{
    public async Task<Result<PagedResult<TripListDto>>> HandleAsync(GetTouristTripsQuery query, CancellationToken cancellationToken)
    {
        var pagedResult = await queryService.GetByTouristIdAsync(userContext.Id, query.Paging, cancellationToken);

        var localizedData = pagedResult.Data.Select(trip => trip with
        {
            StatusDisplay = enumLocalizer.Localize(trip.Status)
        }).ToList();

        return Result.Success(pagedResult with { Data = localizedData });
    }
}
