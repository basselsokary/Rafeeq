using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.NearestTransportations;

public sealed record GetSiteNearestTransportationsByIdQuery(Guid SiteId) : IQuery<List<AdminSiteNearestTransportationDto>>;

internal sealed class GetSiteNearestTransportationsByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteNearestTransportationsByIdQuery, List<AdminSiteNearestTransportationDto>>
{
    public async Task<Result<List<AdminSiteNearestTransportationDto>>> HandleAsync(GetSiteNearestTransportationsByIdQuery query, CancellationToken cancellationToken)
    {
        var site = await queryService.GetByIdForAdminAsync(query.SiteId, cancellationToken);
        if (site is null)
            return SiteErrors.NotFound(query.SiteId);

        var nearestTransportations = await queryService.GetNearestTransportationAsync(query.SiteId, cancellationToken);
        return Result.Success(nearestTransportations);
    }
}
