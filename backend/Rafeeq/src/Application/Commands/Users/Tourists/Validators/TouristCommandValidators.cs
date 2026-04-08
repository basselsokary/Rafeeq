using FluentValidation;
using Domain.Common.Constants;

namespace Application.Commands.Users.Tourists.Validators;

internal class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(DomainConstants.User.MaxFullNameLength);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(DomainConstants.User.MaxFullNameLength);

        RuleFor(x => x.Nationality)
            .NotEmpty();

        RuleFor(x => x.PreferredLanguage)
            .IsInEnum();
    }
}