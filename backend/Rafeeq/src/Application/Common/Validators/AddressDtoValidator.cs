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
            .MaximumLength(StreetMaxLength)
            .WithMessage($"Street cannot exceed {StreetMaxLength} characters.");

        RuleFor(x => x.Region)
            .NotEmpty()
            .WithMessage("District cannot be empty.")
            .MaximumLength(RegionMaxLength)
            .WithMessage($"District cannot exceed {RegionMaxLength} characters.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("City cannot be empty.")
            .MaximumLength(CityMaxLength)
            .WithMessage($"City cannot exceed {CityMaxLength} characters.");

        RuleFor(x => x.PostalCode)
            .MaximumLength(PostalCodeMaxLength)
            .WithMessage($"Zip code cannot exceed {PostalCodeMaxLength} characters.")
            .When(x => !string.IsNullOrWhiteSpace(x.PostalCode));
    }
}
