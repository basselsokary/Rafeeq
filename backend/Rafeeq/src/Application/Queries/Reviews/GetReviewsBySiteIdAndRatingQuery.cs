using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;

namespace Application.Queries.Reviews;

public record GetReviewsBySiteIdAndRatingQuery(
    Guid SiteId,
    int Rating,
    PagingParameters? Paging = null) : IQuery<PagedResult<ReviewListDto>>;

internal class GetReviewsBySiteIdAndRatingQueryHandler(
    IReviewQueryService queryService) : IQueryHandler<GetReviewsBySiteIdAndRatingQuery, PagedResult<ReviewListDto>>
{
    public async Task<Result<PagedResult<ReviewListDto>>> HandleAsync(GetReviewsBySiteIdAndRatingQuery query, CancellationToken cancellationToken)
    {
        PagedResult<ReviewListDto> pagedResult = await queryService.GetBySiteAndRatingAsync(
                query.SiteId,
                query.Rating,
                query.Paging,
                cancellationToken);
        
        return Result.Success(pagedResult);
    }
}