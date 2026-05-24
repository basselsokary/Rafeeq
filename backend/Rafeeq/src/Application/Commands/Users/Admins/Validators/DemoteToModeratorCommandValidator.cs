using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class DemoteToModeratorCommandValidator : AbstractValidator<DemoteToModeratorCommand>
{
    public DemoteToModeratorCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(errors[UserErrors.IdRequired.Code]);
    }
}
