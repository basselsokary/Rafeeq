using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;
using Domain.Entities.SiteAggregate;

namespace Application.Queries.Sites.LocalizedContents;

public sealed record GetSiteLocalizedContentByIdQuery(Guid SiteId, Guid ContentId) : IQuery<AdminSiteLocalizedContentDto>;

internal sealed class GetSiteLocalizedContentByIdQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteLocalizedContentByIdQuery, AdminSiteLocalizedContentDto>
{
    public async Task<Result<AdminSiteLocalizedContentDto>> HandleAsync(GetSiteLocalizedContentByIdQuery query, CancellationToken cancellationToken)
    {
        var localizedContent = await queryService.GetLocalizedContentByIdAsync(query.SiteId, query.ContentId, cancellationToken);
        if (localizedContent is null)
            return SiteErrors.LocalizedContentNotFound;

        return Result.Success(localizedContent);
    }
}
