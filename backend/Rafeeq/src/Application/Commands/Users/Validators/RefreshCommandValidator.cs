using FluentValidation;
using Domain.Common.Constants;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Commands.Users.Validators;

internal sealed class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .MaximumLength(DomainConstants.RefreshToken.MaxRefreshTokenLength)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);
    }
}
