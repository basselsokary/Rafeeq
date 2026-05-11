using Application.Common.Interfaces.Localization;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(errors[EmailErrors.Required.Code]);
    }
}
