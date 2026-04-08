using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;

namespace Application.Queries.Reviews;

public record GetReviewsByStatusQuery(
    PagingParameters Paging,
    ReviewStatus Status = ReviewStatus.Pending) : IQuery<PagedResult<ReviewListDto>>;

internal class GetReviewsByStatusQueryHandler(
    IReviewQueryService queryService) : IQueryHandler<GetReviewsByStatusQuery, PagedResult<ReviewListDto>>
{
    public async Task<Result<PagedResult<ReviewListDto>>> HandleAsync(GetReviewsByStatusQuery query, CancellationToken cancellationToken)
    {
        PagedResult<ReviewListDto> pagedResult = await queryService.GetByStatusAsync(
            query.Paging,
            query.Status,
            cancellationToken);
        
        return Result.Success(pagedResult);
    }
}
