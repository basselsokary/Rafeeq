using Application.Commands.Sponsors.Images;
using Domain.Entities.SponsorAggregate;
using FluentValidation;

namespace Application.Commands.Sponsors.Validators;

internal class AddSponsorImagesCommandValidator : AbstractValidator<AddSponsorImagesCommand>
{
    public AddSponsorImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.Images)
            .NotEmpty();

        RuleForEach(x => x.Images)
            .SetValidator(new AddSponsorImageDtoValidator());
    }
}

internal class AddSponsorImageDtoValidator : AbstractValidator<AddSponsorImageDto>
{
    public AddSponsorImageDtoValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage(SponsorErrors.ImageUrlRequired.Message);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(SponsorErrors.NegativeDisplayOrder.Message);

        RuleFor(x => x.Caption)
            .Must(caption => caption == null || !string.IsNullOrWhiteSpace(caption));
    }
}

internal class RemoveSponsorImagesCommandValidator : AbstractValidator<RemoveSponsorImagesCommand>
{
    public RemoveSponsorImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(SponsorErrors.ImageIdRequired.Message);
        
        RuleForEach(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(SponsorErrors.ImageIdRequired.Message);
    }
}
