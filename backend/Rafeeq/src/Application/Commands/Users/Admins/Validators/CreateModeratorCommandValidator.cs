using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Common.Constants;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands.Users.Admins.Validators;

internal sealed class CreateModeratorCommandValidator : AbstractValidator<CreateModeratorCommand>
{
    public CreateModeratorCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(errors[UserErrors.FirstNameRequired.Code])
            .MaximumLength(DomainConstants.User.MaxFirstNameLength)
            .WithMessage(errors[UserErrors.FirstNameExceededLength.Code]);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(errors[UserErrors.LastNameRequired.Code])
            .MaximumLength(DomainConstants.User.MaxLastNameLength)
            .WithMessage(errors[UserErrors.LastNameExceededLength.Code]);

        RuleFor(x => x.FullName)
            .NotEmpty()
            .WithMessage(errors[UserErrors.FullNameRequired.Code])
            .MaximumLength(DomainConstants.User.MaxFullNameLength)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);

        RuleFor(x => x.Email)
            .EmailAddress()
            .WithMessage(x => errors[EmailErrors.InvalidFormat(x.Email).Code])
            .NotEmpty()
            .WithMessage(errors[EmailErrors.Required.Code])
            .MaximumLength(Email.MaxEmailLength)
            .WithMessage(errors[EmailErrors.TooLong.Code]);
    }
}
