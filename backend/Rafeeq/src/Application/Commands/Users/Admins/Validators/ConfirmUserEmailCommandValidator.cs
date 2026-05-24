using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class ConfirmUserEmailCommandValidator : AbstractValidator<ConfirmUserEmailCommand>
{
    public ConfirmUserEmailCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(errors[UserErrors.IdRequired.Code]);
    }
}
