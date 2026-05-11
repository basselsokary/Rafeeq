using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.ValueObjects;

namespace Application.Commands.Users.Validators;

internal sealed class ResendEmailVerificationCommandValidator : AbstractValidator<ResendEmailVerificationCommand>
{
    public ResendEmailVerificationCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(errors[EmailErrors.Required.Code])
            .EmailAddress()
            .WithMessage(x => errors[EmailErrors.InvalidFormat(x.Email).Code]);
    }
} 