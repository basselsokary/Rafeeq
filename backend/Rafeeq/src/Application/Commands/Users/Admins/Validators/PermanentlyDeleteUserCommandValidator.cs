using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class PermanentlyDeleteUserCommandValidator : AbstractValidator<PermanentlyDeleteUserCommand>
{
    public PermanentlyDeleteUserCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(errors[UserErrors.IdRequired.Code]);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.ConfirmDeletion)
            .Must(value => value)
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
    }
}
