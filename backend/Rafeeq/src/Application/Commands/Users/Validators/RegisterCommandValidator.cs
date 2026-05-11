using FluentValidation;
using Domain.Common.Constants;
using Domain.ValueObjects;
using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Users.Validators;

internal sealed class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage(errors[UserErrors.UserNameRequired.Code])
            .MaximumLength(DomainConstants.User.MaxUserNameLength)
            .WithMessage(errors[UserErrors.UserNameExceededLength.Code]);

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

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage(errors[EmailErrors.Required.Code])
            .EmailAddress()
            .WithMessage(x => errors.Format(EmailErrors.InvalidFormat(string.Empty).Code, x.Email))
            .MaximumLength(Email.MaxEmailLength)
            .WithMessage(errors[EmailErrors.TooLong.Code]);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage(errors[UserErrors.PasswordRequired.Code])
            .MinimumLength(DomainConstants.User.MinPasswordLength)
            .WithMessage(errors[UserErrors.PasswordTooShort.Code])
            .MaximumLength(DomainConstants.User.MaxPasswordLength)
            .WithMessage(errors[UserErrors.PasswordTooLong.Code]);

        RuleFor(x => x.Nationality)
            .NotEmpty()
            .WithMessage(errors[TouristErrors.NationalityRequired.Code]);
    }
}