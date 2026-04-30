using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Attractions;

namespace Application.Queries.Attractions.LocalizedContents;

public sealed record GetAttractionLocalizedContentsQuery(Guid AttractionId)
    : IQuery<List<AttractionLocalizedContentDto>>;

internal sealed class GetAttractionLocalizedContentsQueryHandler(
    IAttractionQueryService queryService) : IQueryHandler<GetAttractionLocalizedContentsQuery, List<AttractionLocalizedContentDto>>
{
    public async Task<Result<List<AttractionLocalizedContentDto>>> HandleAsync(GetAttractionLocalizedContentsQuery query, CancellationToken cancellationToken)
    {
        var localizedContentDtos = await queryService.GetLocalizedContentsAsync(query.AttractionId, cancellationToken);
    
        return localizedContentDtos;
    }
}