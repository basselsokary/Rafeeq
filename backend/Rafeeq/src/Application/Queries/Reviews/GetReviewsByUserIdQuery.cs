using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;

namespace Application.Queries.Reviews;

public record GetReviewsByUserIdQuery(
    PagingParameters Paging) : IQuery<PagedResult<TouristReviewDto>>;

internal class GetReviewsByUserIdQueryHandler(
    IReviewQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetReviewsByUserIdQuery, PagedResult<TouristReviewDto>>
{
    public async Task<Result<PagedResult<TouristReviewDto>>> HandleAsync(GetReviewsByUserIdQuery query, CancellationToken cancellationToken)
    {
        PagedResult<TouristReviewDto> pagedResult = await queryService.GetByUserIdAsync(
                userContext.Id,
                query.Paging,
                cancellationToken);
        
        return Result.Success(pagedResult);
    }
}