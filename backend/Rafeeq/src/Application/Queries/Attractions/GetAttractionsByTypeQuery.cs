using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Attractions;
using Application.DTOs.Common;
using Domain.Enums;

namespace Application.Queries.Attractions;

public record GetAttractionsByTypeQuery(
    Guid SiteId,
    AttractionType Type,
    PagingParameters Paging) : IQuery<PagedResult<AttractionListDto>>;

internal class GetAttractionsByTypeQueryHandler(
    IAttractionQueryService queryService) : IQueryHandler<GetAttractionsByTypeQuery, PagedResult<AttractionListDto>>
{
    public async Task<Result<PagedResult<AttractionListDto>>> HandleAsync(GetAttractionsByTypeQuery query, CancellationToken cancellationToken)
    {
        PagedResult<AttractionListDto> pagedResult = await queryService.GetBySiteIdAsync(
            query.SiteId,
            query.Type,
            query.Paging,
            cancellationToken);

        return Result.Success(pagedResult);
    }
}