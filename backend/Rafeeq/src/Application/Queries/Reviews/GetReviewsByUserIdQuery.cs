using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Common;
using Application.DTOs.Reviews;

namespace Application.Queries.Reviews;

public sealed record GetReviewsByUserIdQuery(
    PagingParameters Paging) : IQuery<PagedResult<TouristReviewDto>>;

internal sealed class GetReviewsByUserIdQueryHandler(
    IReviewQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetReviewsByUserIdQuery, PagedResult<TouristReviewDto>>
{
    public async Task<Result<PagedResult<TouristReviewDto>>> HandleAsync(GetReviewsByUserIdQuery query, CancellationToken cancellationToken)
    {
        PagedResult<TouristReviewDto> pagedResult = await queryService.GetByTouristIdAsync(
            userContext.Id,
            query.Paging,
            userContext.Language,
            cancellationToken);
        
        var localizedData = pagedResult.Data.Select(review => review with
        {
            SiteTypeDisplay = enumLocalizer.Localize(review.SiteType),
            StatusDisplay = enumLocalizer.Localize(review.Status)
        }).ToList();

        return Result.Success(pagedResult with { Data = localizedData });
    }
}