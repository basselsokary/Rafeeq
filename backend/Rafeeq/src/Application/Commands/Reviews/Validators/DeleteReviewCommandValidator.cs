using Domain.Entities.ReviewAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Reviews.Validators;

internal sealed class DeleteReviewCommandValidator : AbstractValidator<DeleteReviewCommand>
{
    public DeleteReviewCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ReviewErrors.IdRequired.Code]);
    }
}
