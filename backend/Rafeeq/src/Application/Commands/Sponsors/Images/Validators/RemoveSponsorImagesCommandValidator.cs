using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.SponsorAggregate;
using FluentValidation;

namespace Application.Commands.Sponsors.Images.Validators;

internal sealed class RemoveSponsorImagesCommandValidator : AbstractValidator<RemoveSponsorImagesCommand>
{
    public RemoveSponsorImagesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
        
        RuleFor(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);
        
        RuleForEach(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.ImageIdRequired.Code]);
    }
}

