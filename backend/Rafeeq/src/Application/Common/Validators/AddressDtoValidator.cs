using Application.DTOs.Common;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Address;

namespace Application.Common.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street cannot be empty.")
            .MaximumLength(MaxStreetLength)
            .WithMessage($"Street cannot exceed {MaxStreetLength} characters.");

        RuleFor(x => x.Region)
            .NotEmpty()
            .WithMessage("District cannot be empty.")
            .MaximumLength(MaxRegionLength)
            .WithMessage($"District cannot exceed {MaxRegionLength} characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City cannot be empty.")
            .MaximumLength(MaxCityLength)
            .WithMessage($"City cannot exceed {MaxCityLength} characters.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(MaxPostalCodeLength)
            .WithMessage($"Zip code cannot exceed {MaxPostalCodeLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));
    }
}
