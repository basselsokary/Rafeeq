using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.OpeningHours;

public sealed record GetSiteOpeningHoursByIdQuery(Guid SiteId) : IQuery<List<AdminSiteOpeningHourDto>>;

internal sealed class GetSiteOpeningHoursByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteOpeningHoursByIdQuery, List<AdminSiteOpeningHourDto>>
{
    public async Task<Result<List<AdminSiteOpeningHourDto>>> HandleAsync(GetSiteOpeningHoursByIdQuery query, CancellationToken cancellationToken)
    {
        var site = await queryService.GetByIdForAdminAsync(query.SiteId, cancellationToken);
        if (site is null)
            return SiteErrors.NotFound(query.SiteId);

        var openingHours = await queryService.GetOpeningHoursAsync(query.SiteId, cancellationToken);
        return Result.Success(openingHours);
    }
}
