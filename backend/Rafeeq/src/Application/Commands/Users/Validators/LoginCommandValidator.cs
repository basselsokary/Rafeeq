using FluentValidation;
using Domain.Common.Constants;
using Domain.ValueObjects;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Commands.Users.Validators;

internal sealed class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(errors[EmailErrors.Required.Code])
            .EmailAddress()
            .WithMessage(x => errors.Format(EmailErrors.InvalidFormat(string.Empty).Code, x.Email))
            .MaximumLength(Email.MaxEmailLength)
            .WithMessage(errors[EmailErrors.TooLong.Code]);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(errors[UserErrors.PasswordRequired.Code])
            .MaximumLength(DomainConstants.User.MaxPasswordLength)
            .WithMessage(errors[UserErrors.PasswordTooLong.Code]);
    }
}
