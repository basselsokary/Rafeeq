using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Localization;
using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Reviews;
using Domain.Entities.ReviewAggregate;

namespace Application.Queries.Reviews;

public sealed record GetReviewByIdQuery(
    Guid Id) : IQuery<ReviewDetailDto>;

internal sealed class GetReviewByIdQueryHandler(
    IReviewQueryService queryService,
    IUserContext userContext,
    IEnumLocalizer enumLocalizer) : IQueryHandler<GetReviewByIdQuery, ReviewDetailDto>
{
    public async Task<Result<ReviewDetailDto>> HandleAsync(GetReviewByIdQuery query, CancellationToken cancellationToken)
    {
        ReviewDetailDto? reviewDetailDto = await queryService.GetByIdAsync(query.Id, userContext.Language, cancellationToken);
        if (reviewDetailDto == null)
            return ReviewErrors.NotFound(query.Id);

        return Result.Success(reviewDetailDto with 
        {
            SiteTypeDisplay = enumLocalizer.Localize(reviewDetailDto.SiteType),
            StatusDisplay = enumLocalizer.Localize(reviewDetailDto.Status)
        });
    }
}