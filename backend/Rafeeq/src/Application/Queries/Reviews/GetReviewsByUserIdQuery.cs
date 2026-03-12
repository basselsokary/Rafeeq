using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Messaging;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;

namespace Application.Queries.Reviews;

public record GetReviewsByUserIdQuery(
    PagingParameters? Paging = null) : IQuery<PagedResult<UserReviewDto>>;

internal class GetReviewsByUserIdQueryHandler(
    IReviewQueryService queryService,
    IUserContext userContext) : IQueryHandler<GetReviewsByUserIdQuery, PagedResult<UserReviewDto>>
{
    public async Task<Result<PagedResult<UserReviewDto>>> HandleAsync(GetReviewsByUserIdQuery query, CancellationToken cancellationToken)
    {
        PagedResult<UserReviewDto> pagedResult = await queryService.GetByUserIdAsync(
                userContext.Id,
                query.Paging,
                cancellationToken);
        
        return Result.Success(pagedResult);
    }
}