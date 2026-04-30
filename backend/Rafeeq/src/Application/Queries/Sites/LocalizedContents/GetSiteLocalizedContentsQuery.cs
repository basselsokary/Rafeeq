using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Admins;

namespace Application.Queries.Sites.LocalizedContents;

public sealed record GetSiteLocalizedContentsQuery(Guid SiteId) : IQuery<List<AdminSiteLocalizedContentDto>>;

internal sealed class GetSiteLocalizedContentsQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteLocalizedContentsQuery, List<AdminSiteLocalizedContentDto>>
{
    public async Task<Result<List<AdminSiteLocalizedContentDto>>> HandleAsync(GetSiteLocalizedContentsQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDtos = await queryService.GetLocalizedContentsAsync(query.SiteId, cancellationToken);
    
        return localizedContentDtos;
    }
}