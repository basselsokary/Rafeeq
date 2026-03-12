using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;

namespace Application.Queries.Reviews;

public record GetReviewsBySiteIdQuery(
    Guid SiteId,
    bool Approved = true,
    string? SortBy = "Helpful", // Helpful, Recent, Rating
    PagingParameters? Paging = null) : IQuery<PagedResult<ReviewListDto>>;

internal class GetReviewsBySiteIdQueryHandler(
    IReviewQueryService queryService) : IQueryHandler<GetReviewsBySiteIdQuery, PagedResult<ReviewListDto>>
{
    public async Task<Result<PagedResult<ReviewListDto>>> HandleAsync(GetReviewsBySiteIdQuery query, CancellationToken cancellationToken)
    {
        PagedResult<ReviewListDto> pagedResult;
        if (query.Approved)
        {
            pagedResult = await queryService.GetApprovedBySiteIdAsync(
                query.SiteId,
                query.SortBy,
                query.Paging,
                cancellationToken);
        }
        else
        {
            pagedResult = await queryService.GetBySiteIdAsync(
                query.SiteId,
                query.SortBy,
                query.Paging,
                cancellationToken);
        }
        
        return Result.Success(pagedResult);
    }
}