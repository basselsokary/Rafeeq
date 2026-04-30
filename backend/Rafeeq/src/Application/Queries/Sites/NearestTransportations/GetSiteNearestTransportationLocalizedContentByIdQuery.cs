using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.NearestTransportations;

public sealed record GetSiteNearestTransportationLocalizedContentByIdQuery(
    Guid SiteId,
    Guid TransportationId,
    Guid ContentId) : IQuery<AdminSiteNearestTransportationLocalizedContentDto>;

internal sealed class GetSiteNearestTransportationLocalizedContentByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteNearestTransportationLocalizedContentByIdQuery, AdminSiteNearestTransportationLocalizedContentDto>
{
    public async Task<Result<AdminSiteNearestTransportationLocalizedContentDto>> HandleAsync(
        GetSiteNearestTransportationLocalizedContentByIdQuery query,
        CancellationToken cancellationToken)
    {
        var localizedContent = await queryService.GetNearestTransportationLocalizedContentByIdAsync(
            query.SiteId,
            query.TransportationId,
            query.ContentId,
            cancellationToken);

        if (localizedContent is null)
            return SiteErrors.NearestTransportationLocalizedContentNotFound;

        return Result.Success(localizedContent);
    }
}
