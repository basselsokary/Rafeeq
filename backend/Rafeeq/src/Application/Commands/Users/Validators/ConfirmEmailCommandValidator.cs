using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Commands.Users.Validators;

internal sealed class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
    }
}