using FluentValidation;
using Domain.Common.Constants;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Commands.Users.Validators;

internal sealed class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Token)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage(errors[UserErrors.PasswordRequired.Code])
            .MinimumLength(DomainConstants.User.MinPasswordLength)
            .WithMessage(errors[UserErrors.PasswordTooShort.Code])
            .MaximumLength(DomainConstants.User.MaxPasswordLength)
            .WithMessage(errors[UserErrors.PasswordTooLong.Code]);
    }
}
