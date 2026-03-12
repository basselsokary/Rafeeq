using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Sites;

namespace Application.Queries.Sites.LocalizedContents;

public record GetSiteLocalizedContentsQuery(Guid SiteId) : IQuery<List<LocalizedContentDto>>;

internal class GetSiteLocalizedContentsQueryHandler(
    ISiteQueryService queryService) : IQueryHandler<GetSiteLocalizedContentsQuery, List<LocalizedContentDto>>
{
    public async Task<Result<List<LocalizedContentDto>>> HandleAsync(GetSiteLocalizedContentsQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDtos = await queryService.GetLocalizedContentsAsync(query.SiteId, cancellationToken);
    
        return localizedContentDtos;
    }
}