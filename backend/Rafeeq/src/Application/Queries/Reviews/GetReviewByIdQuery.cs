using Application.Common.Interfaces.QueryServices;
using Application.DTOs.Reviews;
using Domain.Entities.ReviewAggregate;

namespace Application.Queries.Reviews;

public record GetReviewByIdQuery(
    Guid Id) : IQuery<ReviewDetailDto>;

internal class GetReviewByIdQueryHandler(
    IReviewQueryService queryService) : IQueryHandler<GetReviewByIdQuery, ReviewDetailDto>
{
    public async Task<Result<ReviewDetailDto>> HandleAsync(GetReviewByIdQuery query, CancellationToken cancellationToken)
    {
        ReviewDetailDto? reviewDetailDto = await queryService.GetByIdAsync(query.Id, cancellationToken);
        if (reviewDetailDto == null)
            return ReviewErrors.NotFound(query.Id);

        return Result.Success(reviewDetailDto);
    }
}