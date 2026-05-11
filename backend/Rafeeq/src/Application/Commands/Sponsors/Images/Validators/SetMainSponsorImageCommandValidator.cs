using Application.Common.Interfaces.Localization;
using Domain.Entities.SponsorAggregate;
using FluentValidation;

namespace Application.Commands.Sponsors.Images.Validators;

internal sealed class SetMainSponsorImageCommandValidator : AbstractValidator<SetMainSponsorImageCommand>
{
    public SetMainSponsorImageCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
        
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.ImageIdRequired.Code]);
    }
}

