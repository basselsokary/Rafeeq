using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Trips;
using Domain.Entities.TripAggregate;

namespace Application.Queries.Trips;

public sealed record GetTripByIdQuery(Guid Id) : IQuery<TripDetailDto>;

internal sealed class GetTripByIdQueryHandler(
    ITripQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetTripByIdQuery, TripDetailDto>
{
    public async Task<Result<TripDetailDto>> HandleAsync(GetTripByIdQuery query, CancellationToken cancellationToken)
    {
        var tripDto = await queryService.GetByIdAsync(query.Id, userContext.Id, cancellationToken);
        if (tripDto == null)
            return TripErrors.NotFound;

        var localizedDays = tripDto.Days.Select(day => day with
        {
            Sites = day.Sites.Select(site => site with
            {
                SiteTypeDisplay = enumLocalizer.Localize(site.SiteType)
            }).ToList()
        }).ToList();

        var preferredSiteTypesDisplay = tripDto.PreferredSiteTypes
            .Select(siteType => enumLocalizer.Localize(siteType))
            .ToList();

        return Result.Success(tripDto with
        {
            Days = localizedDays,
            PreferredSiteTypesDisplay = preferredSiteTypesDisplay,
            StatusDisplay = enumLocalizer.Localize(tripDto.Status),
            ToleranceDisplay = tripDto.Tolerance.HasValue
                ? enumLocalizer.Localize(tripDto.Tolerance.Value)
                : string.Empty
        });
    }
}
