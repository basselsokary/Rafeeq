using FluentValidation;

namespace Application.Features.Reviews.Queires;

public record GetReviewByIdQuery(
) : IQuery<GetReviewByIdResponse>;

internal class GetReviewByIdQueryHandler : IQueryHandler<GetReviewByIdQuery, GetReviewByIdResponse>
{
    public Task<Result<GetReviewByIdResponse>> HandleAsync(GetReviewByIdQuery query, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}

internal class GetReviewByIdQueryValidator : AbstractValidator<GetReviewByIdQuery>
{
    public GetReviewByIdQueryValidator()
    {
        throw new NotImplementedException();
    }
}

public record GetReviewByIdResponse();