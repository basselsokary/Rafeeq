using FluentValidation;
using Application.Queries.Common.Validators;
using Domain.Entities.ReviewAggregate;
using Domain.ValueObjects;
using Application.Common.Interfaces.Localization;
using Domain.Common;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Application.Queries.Reviews.Validators;

internal sealed class GetReviewsBySiteIdQueryValidator : AbstractValidator<GetReviewsBySiteIdQuery>
{
    public GetReviewsBySiteIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[ReviewErrors.SiteIdRequired.Code]);

        RuleFor(x => x.Paging)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new PagingParametersValidator(errors));

        RuleFor(x => x.Rating)
            .InclusiveBetween(MinRatingValue, MaxRatingValue)
            .When(x => x.Rating.HasValue)
            .WithMessage(errors.Format(RatingErrors.OutOfRange(MinRatingValue, MaxRatingValue).Code, MinRatingValue, MaxRatingValue));

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Status.ToString()));

        RuleFor(x => x.OrderBy)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.OrderBy.ToString()));
    }
}
