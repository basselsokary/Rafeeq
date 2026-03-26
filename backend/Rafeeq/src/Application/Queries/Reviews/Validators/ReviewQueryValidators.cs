using FluentValidation;
using Application.Queries.Common.Validators;
using Domain.Common.Constants;
using Domain.Entities.ReviewAggregate;

namespace Application.Queries.Reviews.Validators;

internal class GetReviewsBySiteIdQueryValidator : AbstractValidator<GetReviewsBySiteIdQuery>
{
    public GetReviewsBySiteIdQueryValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(ReviewErrors.SiteIdRequired.Message);

        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());

        RuleFor(x => x.Rating)
            .InclusiveBetween(DomainConstants.Review.MinRatingValue, DomainConstants.Review.MaxRatingValue)
            .When(x => x.Rating.HasValue);

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.OrderBy)
            .IsInEnum();
    }
}

internal class GetReviewByIdQueryValidator : AbstractValidator<GetReviewByIdQuery>
{
    public GetReviewByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(ReviewErrors.IdRequired.Message);
    }
}

internal class GetReviewsByUserIdQueryValidator : AbstractValidator<GetReviewsByUserIdQuery>
{
    public GetReviewsByUserIdQueryValidator()
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());
    }
}

internal class GetReviewsByStatusQueryValidator : AbstractValidator<GetReviewsByStatusQuery>
{
    public GetReviewsByStatusQueryValidator()
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());

        RuleFor(x => x.Status)
            .IsInEnum();
    }
}
