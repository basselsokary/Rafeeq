using FluentValidation;
using Domain.Common.Constants;
using Domain.Entities.TouristAggregate;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Users.Tourists.Validators;

internal sealed class UpdateProfileCommandValidator : AbstractValidator<UpdateProfileCommand>
{
    public UpdateProfileCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage(errors[TouristErrors.FirstNameRequired.Code])
            .MaximumLength(DomainConstants.Tourist.MaxFullNameLength)
            .WithMessage(errors[TouristErrors.FirstNameExceededLength.Code]);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage(errors[TouristErrors.LastNameRequired.Code])
            .MaximumLength(DomainConstants.Tourist.MaxFullNameLength)
            .WithMessage(errors[TouristErrors.LastNameExceededLength.Code]);

        RuleFor(x => x.Nationality)
            .NotEmpty()
            .WithMessage(errors[TouristErrors.NationalityRequired.Code]);
    }
}