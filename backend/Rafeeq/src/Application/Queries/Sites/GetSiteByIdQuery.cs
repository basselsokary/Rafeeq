using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites;

public record GetSiteByIdQuery(Guid Id) : IQuery<SiteDetailDto>;

internal class GetSiteByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteByIdQuery, SiteDetailDto>
{
    public async Task<Result<SiteDetailDto>> HandleAsync(GetSiteByIdQuery query, CancellationToken cancellationToken)
    {
        var siteDetailDto = await queryService.GetByIdAsync(query.Id, cancellationToken);
        if (siteDetailDto == null)
            return SiteErrors.NotFound(query.Id);

        return Result.Success(siteDetailDto);
    }
}