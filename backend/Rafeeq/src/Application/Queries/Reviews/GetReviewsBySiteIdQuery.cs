using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;
using Domain.Enums;

namespace Application.Queries.Reviews;

public sealed record GetReviewsBySiteIdQuery(
    Guid SiteId,
    PagingParameters Paging,
    int? Rating = null,
    ReviewStatus Status = ReviewStatus.Approved,
    ReviewOrderBy OrderBy = ReviewOrderBy.Helpful) : IQuery<PagedResult<ReviewListDto>>;

internal sealed class GetReviewsBySiteIdQueryHandler(
    IReviewQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetReviewsBySiteIdQuery, PagedResult<ReviewListDto>>
{
    public async Task<Result<PagedResult<ReviewListDto>>> HandleAsync(GetReviewsBySiteIdQuery query, CancellationToken cancellationToken)
    {
        PagedResult<ReviewListDto> pagedResult = await queryService.GetBySiteIdAsync(
            query.SiteId,
            query.Paging,
            query.Rating,
            query.OrderBy,
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