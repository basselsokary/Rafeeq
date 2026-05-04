using Domain.Entities.ReviewAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.ValueObjects;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Application.Commands.Reviews.Validators;

internal sealed class CreateReviewCommandValidator : AbstractValidator<CreateReviewCommand>
{
    public CreateReviewCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[ReviewErrors.SiteIdRequired.Code]);
        
        RuleFor(x => x.Rating)
            .GreaterThan(0)
            .WithMessage(errors[RatingErrors.PositiveRatingRequired.Code]);
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(errors[ReviewErrors.TitleRequired.Code])
            .MaximumLength(MaxTitleLength)
            .WithMessage(errors.Format(ReviewErrors.ExeededTitleLength.Code, MaxTitleLength));
        
        RuleFor(x => x.Content)
            .NotEmpty()
            .WithMessage(errors[ReviewErrors.ContentRequired.Code])
            .MaximumLength(MaxContentLength)
            .WithMessage(errors.Format(ReviewErrors.ExeededContentLength.Code, MaxContentLength));
    }
}
