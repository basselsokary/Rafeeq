using Application.DTOs;
using FluentValidation;
using static Domain.Constants.DomainConstants.Address;

namespace Application.Common.Validators;

public class AddressDtoValidator : AbstractValidator<AddressDto>
{
    public AddressDtoValidator()
    {
        RuleFor(x => x.Street)
            .NotEmpty()
            .WithMessage("Street cannot be empty.")
            .MaximumLength(StreetMaxLength)
            .WithMessage($"Street cannot exceed {StreetMaxLength} characters.");

        RuleFor(x => x.District)
            .NotEmpty()
            .WithMessage("District cannot be empty.")
            .MaximumLength(DistrictMaxLength)
            .WithMessage($"District cannot exceed {DistrictMaxLength} characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City cannot be empty.")
            .MaximumLength(CityMaxLength)
            .WithMessage($"City cannot exceed {CityMaxLength} characters.");

        RuleFor(x => x.ZipCode)
            .MaximumLength(ZipCodeMaxLength)
            .WithMessage($"Zip code cannot exceed {ZipCodeMaxLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.ZipCode));
    }
}
