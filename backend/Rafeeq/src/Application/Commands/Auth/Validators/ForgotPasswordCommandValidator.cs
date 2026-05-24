using FluentValidation;
using Domain.ValueObjects;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Auth.Validators;

internal sealed class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(errors[EmailErrors.Required.Code])
            .EmailAddress()
            .WithMessage(x => errors.Format(EmailErrors.InvalidFormat(string.Empty).Code, x.Email))
            .MaximumLength(Email.MaxEmailLength)
            .WithMessage(errors[EmailErrors.TooLong.Code]);
    }
}
