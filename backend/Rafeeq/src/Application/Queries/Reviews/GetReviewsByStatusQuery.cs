using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;

namespace Application.Queries.Reviews;

public sealed record GetReviewsByStatusQuery(
    PagingParameters Paging,
    ReviewStatus Status = ReviewStatus.Pending) : IQuery<PagedResult<ReviewListDto>>;

internal sealed class GetReviewsByStatusQueryHandler(
    IReviewQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetReviewsByStatusQuery, PagedResult<ReviewListDto>>
{
    public async Task<Result<PagedResult<ReviewListDto>>> HandleAsync(GetReviewsByStatusQuery query, CancellationToken cancellationToken)
    {
        PagedResult<ReviewListDto> pagedResult = await queryService.GetByStatusAsync(
            query.Paging,
            query.Status,
            userContext.Language,
            cancellationToken);

        var localizedData = pagedResult.Data.Select(review => review with
        {
            StatusDisplay = enumLocalizer.Localize(review.Status)
        }).ToList();
        
        return Result.Success(pagedResult with { Data = localizedData });
    }
}
