using Application.DTOs.Sites;
using Domain.ValueObjects;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Review;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Common.Validators;

internal sealed class SiteFiltersValidator : AbstractValidator<SiteFilters>
{
    public SiteFiltersValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue)
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type.ToString() ?? "null"));

        RuleFor(x => x.City)
            .NotEmpty()
            .When(x => x.City.HasValue)
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.MinRating)
            .InclusiveBetween(MinRatingValue, MaxRatingValue)
            .When(x => x.MinRating.HasValue)
            .WithMessage(errors.Format(RatingErrors.OutOfRange(MinRatingValue, MaxRatingValue).Code, MinRatingValue, MaxRatingValue));

        RuleFor(x => x.MaxRating)
            .InclusiveBetween(MinRatingValue, MaxRatingValue)
            .When(x => x.MaxRating.HasValue)
            .WithMessage(errors.Format(RatingErrors.OutOfRange(MinRatingValue, MaxRatingValue).Code, MinRatingValue, MaxRatingValue));

        RuleFor(x => x)
            .Must(filters => !filters.MinRating.HasValue || !filters.MaxRating.HasValue || filters.MinRating <= filters.MaxRating)
            .WithMessage(errors.Format(RatingErrors.OutOfRange(MinRatingValue, MaxRatingValue).Code, MinRatingValue, MaxRatingValue));
    }
}
