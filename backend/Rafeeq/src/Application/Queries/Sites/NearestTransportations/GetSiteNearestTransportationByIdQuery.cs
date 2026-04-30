using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.NearestTransportations;

public sealed record GetSiteNearestTransportationByIdQuery(Guid SiteId, Guid TransportationId) : IQuery<AdminSiteNearestTransportationDto>;

internal sealed class GetSiteNearestTransportationByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteNearestTransportationByIdQuery, AdminSiteNearestTransportationDto>
{
    public async Task<Result<AdminSiteNearestTransportationDto>> HandleAsync(GetSiteNearestTransportationByIdQuery query, CancellationToken cancellationToken)
    {
        var nearestTransportation = await queryService.GetNearestTransportationByIdAsync(
            query.SiteId,
            query.TransportationId,
            cancellationToken);

        if (nearestTransportation is null)
            return SiteErrors.TransportationNotFound;

        return Result.Success(nearestTransportation);
    }
}
