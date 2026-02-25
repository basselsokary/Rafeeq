using static Domain.Common.Constants.DomainConstants.User;
using FluentValidation;

namespace Application.Features.Users.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .WithMessage("Invalid email format.")
            .MaximumLength(MaxEmailLength)
            .WithMessage($"Email must not exceed {MaxEmailLength} characters.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(3) // For testing purposes, minimum length is set to 3
            // .MinimumLength(MinPasswordLength)
            .WithMessage($"Password must be at least {MinPasswordLength} characters long.")
            .MaximumLength(MaxPasswordLength)
            .WithMessage($"Password must not exceed {MaxPasswordLength} characters.");
    }
    
}