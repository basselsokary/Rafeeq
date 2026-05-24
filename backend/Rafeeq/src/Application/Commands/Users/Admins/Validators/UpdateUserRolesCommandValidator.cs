using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class UpdateUserRolesCommandValidator : AbstractValidator<UpdateUserRolesCommand>
{
    public UpdateUserRolesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage(errors[UserErrors.IdRequired.Code]);

        RuleFor(x => x.Roles)
            .NotNull()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code])
            .Must(roles => roles.Count > 0)
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.Roles)
            .Must(role => !string.IsNullOrWhiteSpace(role))
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
    }
}
