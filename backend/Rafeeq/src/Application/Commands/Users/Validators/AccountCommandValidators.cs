using FluentValidation;
using Domain.Common.Constants;

namespace Application.Commands.Users.Validators;

public class ConfirmEmailCommandValidator : AbstractValidator<ConfirmEmailCommand>
{
    public ConfirmEmailCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
    }
}

public class ForgotPasswordCommandValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(DomainConstants.User.MaxEmailLength);
    }
}

public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(DomainConstants.User.MinPasswordLength)
            .MaximumLength(DomainConstants.User.MaxPasswordLength);
    }
}

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(DomainConstants.User.MaxEmailLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MaximumLength(DomainConstants.User.MaxPasswordLength);
    }
}

public class RefreshCommandValidator : AbstractValidator<RefreshCommand>
{
    public RefreshCommandValidator()
    {
        RuleFor(x => x.AccessToken)
            .NotEmpty();

        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .MaximumLength(DomainConstants.RefreshToken.MaxRefreshTokenLength);
    }
}

public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.UserName)
            .NotEmpty()
            .MaximumLength(DomainConstants.User.MaxUserNameLength);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(DomainConstants.User.MaxFirstNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(DomainConstants.User.MaxLastNameLength);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(DomainConstants.User.MaxEmailLength);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(DomainConstants.User.MinPasswordLength)
            .MaximumLength(DomainConstants.User.MaxPasswordLength);

        RuleFor(x => x.Nationality)
            .NotEmpty();

        RuleFor(x => x.PreferredLanguage)
            .IsInEnum();
    }
}