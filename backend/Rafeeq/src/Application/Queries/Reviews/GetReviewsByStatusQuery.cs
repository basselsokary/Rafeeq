using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;

namespace Application.Queries.Reviews;

public record GetReviewsByStatusQuery(
    ReviewStatus Status = ReviewStatus.Pending,
    PagingParameters? Paging = null) : IQuery<PagedResult<ReviewListDto>>;

internal class GetReviewsByStatusQueryHandler(
    IReviewQueryService queryService) : IQueryHandler<GetReviewsByStatusQuery, PagedResult<ReviewListDto>>
{
    public async Task<Result<PagedResult<ReviewListDto>>> HandleAsync(GetReviewsByStatusQuery query, CancellationToken cancellationToken)
    {
        PagedResult<ReviewListDto> pagedResult = await queryService.GetByStatusAsync(
            query.Status,
            query.Paging,
            cancellationToken);
        
        return Result.Success(pagedResult);
    }
}
