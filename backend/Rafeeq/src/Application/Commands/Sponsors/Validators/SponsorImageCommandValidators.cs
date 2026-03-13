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
        
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage(SponsorErrors.ImageUrlRequired.Message);
    }
}

internal class RemoveSponsorImagesCommandValidator : AbstractValidator<RemoveSponsorImagesCommand>
{
    public RemoveSponsorImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage(SponsorErrors.ImageUrlRequired.Message);
    }
}
