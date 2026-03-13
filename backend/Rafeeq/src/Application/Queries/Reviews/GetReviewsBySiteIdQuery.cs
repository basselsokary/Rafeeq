using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;

namespace Application.Queries.Reviews;

public record GetReviewsBySiteIdQuery(
    Guid SiteId,
    PagingParameters Paging,
    ReviewStatus Status = ReviewStatus.Approved,
    ReviewOrderBy OrderBy = ReviewOrderBy.Helpful) : IQuery<PagedResult<ReviewListDto>>;

internal class GetReviewsBySiteIdQueryHandler(
    IReviewQueryService queryService) : IQueryHandler<GetReviewsBySiteIdQuery, PagedResult<ReviewListDto>>
{
    public async Task<Result<PagedResult<ReviewListDto>>> HandleAsync(GetReviewsBySiteIdQuery query, CancellationToken cancellationToken)
    {
        PagedResult<ReviewListDto> pagedResult = await queryService.GetBySiteIdAsync(
            query.SiteId,
            query.Paging,
            query.OrderBy,
            query.Status,
            cancellationToken);
        
        return Result.Success(pagedResult);
    }
}