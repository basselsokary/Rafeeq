using Domain.Entities.ReviewAggregate;
using Domain.Enums;
using FluentValidation;
using Domain.Common;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Reviews.Validators;

internal sealed class SetReviewStatusCommandValidator : AbstractValidator<SetReviewStatusCommand>
{
    public SetReviewStatusCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ReviewErrors.IdRequired.Code]);
        
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Status.ToString()));
        
        RuleFor(x => x.RejectionReason)
            .NotEmpty()
            .When(x => x.Status == ReviewStatus.Rejected)
            .WithMessage(errors[ReviewErrors.RejectionReasonRequired.Code]);
    }
}
