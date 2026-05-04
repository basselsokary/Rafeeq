using Domain.Entities.ReviewAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.ValueObjects;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Application.Commands.Reviews.Validators;

internal sealed class UpdateReviewCommandValidator : AbstractValidator<UpdateReviewCommand>
{
    public UpdateReviewCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ReviewErrors.IdRequired.Code]);
        
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
