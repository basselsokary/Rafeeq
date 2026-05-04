using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Commands.Users.Tourists.Validators;

internal sealed class LoginWithGoogleCommandValidator : AbstractValidator<LoginWithGoogleCommand>
{
    public LoginWithGoogleCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.IdToken)
            .NotEmpty()
            .WithMessage(errors[UserErrors.GoogleIdTokenRequired.Code]);
    }
}