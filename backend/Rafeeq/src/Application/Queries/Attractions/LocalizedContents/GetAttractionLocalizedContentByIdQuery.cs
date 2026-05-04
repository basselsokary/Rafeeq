using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Attractions;
using Domain.Entities.AttractionAggregate;

namespace Application.Queries.Attractions.LocalizedContents;

public sealed record GetAttractionLocalizedContentByIdQuery(Guid AttractionId, Guid ContentId) : IQuery<AttractionLocalizedContentDto>;

internal sealed class GetAttractionLocalizedContentByIdQueryHandler(
    IAttractionQueryService queryService) : IQueryHandler<GetAttractionLocalizedContentByIdQuery, AttractionLocalizedContentDto>
{
    public async Task<Result<AttractionLocalizedContentDto>> HandleAsync(GetAttractionLocalizedContentByIdQuery query, CancellationToken cancellationToken)
    {
        var localizedContent = await queryService.GetLocalizedContentByIdAsync(query.AttractionId, query.ContentId, cancellationToken);
        if (localizedContent is null)
            return AttractionErrors.LocalizedContentNotFound;

        return Result.Success(localizedContent);
    }
}
