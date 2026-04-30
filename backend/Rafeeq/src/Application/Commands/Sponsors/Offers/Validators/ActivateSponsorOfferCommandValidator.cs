using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Sponsors.Offers.Validators;

internal sealed class ActivateSponsorOfferCommandValidator : AbstractValidator<ActivateSponsorOfferCommand>
{
    public ActivateSponsorOfferCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);

        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.OfferIdRequired.Code]);
    }
}
