using Application.Common.Interfaces.Localization;
using Domain.Common;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class CreateModeratorCommandValidator : AbstractValidator<CreateModeratorCommand>
{
    public CreateModeratorCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(errors[UserErrors.FullNameRequired.Code]);
        
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(errors[UserErrors.FirstNameRequired.Code]);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(errors[UserErrors.LastNameRequired.Code]);
    }
}
