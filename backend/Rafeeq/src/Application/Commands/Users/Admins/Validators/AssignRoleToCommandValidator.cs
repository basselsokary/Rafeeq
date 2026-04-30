using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class AssignRoleToCommandValidator : AbstractValidator<AssignRoleToCommand>
{
    public AssignRoleToCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserId)
            .NotEqual(Guid.Empty)
            .WithMessage(errors[UserErrors.IdRequired.Code]);

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage(errors[UserErrors.RoleInvalid.Code]);
    }
}
