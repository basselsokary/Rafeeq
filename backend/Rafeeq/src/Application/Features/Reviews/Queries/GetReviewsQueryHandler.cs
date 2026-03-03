using FluentValidation;

namespace Application.Features.Reviews.Queires;

public record GetReviewsQuery(
) : IQuery<GetReviewsResponse>;

internal class GetReviewsQueryHandler : IQueryHandler<GetReviewsQuery, GetReviewsResponse>
{
    public Task<Result<GetReviewsResponse>> HandleAsync(GetReviewsQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetReviewsQueryValidator : AbstractValidator<GetReviewsQuery>
{
    public GetReviewsQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetReviewsResponse();