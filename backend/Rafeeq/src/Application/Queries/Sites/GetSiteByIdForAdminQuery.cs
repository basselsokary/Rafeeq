using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites;

public record GetSiteByIdForAdminQuery(Guid Id) : IQuery<SiteAdminDetailDto>;

internal class GetSiteByIdForAdminQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteByIdForAdminQuery, SiteAdminDetailDto>
{
    public async Task<Result<SiteAdminDetailDto>> HandleAsync(GetSiteByIdForAdminQuery query, CancellationToken cancellationToken)
    {
        var siteDetailDto = await queryService.GetByIdForAdminAsync(query.Id, cancellationToken);
        if (siteDetailDto == null)
            return SiteErrors.NotFound(query.Id);

        return Result.Success(siteDetailDto);
    }
}