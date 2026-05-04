using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Commands.Users.Moderators.Validators;

public sealed class ActivateTouristCommandValidator : AbstractValidator<ActivateTouristCommand>
{
    public ActivateTouristCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.TouristId)
            .NotEmpty()
            .WithMessage(errors[UserErrors.IdRequired.Code]);
    }
}
