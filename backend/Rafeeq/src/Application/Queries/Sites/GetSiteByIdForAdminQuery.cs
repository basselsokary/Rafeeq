using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites;

public sealed record GetSiteByIdForAdminQuery(Guid Id) : IQuery<AdminSiteDetailDto>;

internal sealed class GetSiteByIdForAdminQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteByIdForAdminQuery, AdminSiteDetailDto>
{
    public async Task<Result<AdminSiteDetailDto>> HandleAsync(GetSiteByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var siteDetailDto = await queryService.GetByIdForAdminAsync(query.Id, cancellationToken);
        if (siteDetailDto == null)
            return SiteErrors.NotFound(query.Id);

        return Result.Success(siteDetailDto);
    }
}