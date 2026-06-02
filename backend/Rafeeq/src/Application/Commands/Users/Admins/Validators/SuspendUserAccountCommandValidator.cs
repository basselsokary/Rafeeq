using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class SuspendUserAccountCommandValidator : AbstractValidator<SuspendUserAccountCommand>
{
    public SuspendUserAccountCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(errors[UserErrors.IdRequired.Code]);

        RuleFor(x => x.Reason)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.SuspendUntil)
            .Must(date => date > DateTime.UtcNow)
            .WithMessage(errors[ValidationErrors.RangeInvalid().Code]);
    }
}
