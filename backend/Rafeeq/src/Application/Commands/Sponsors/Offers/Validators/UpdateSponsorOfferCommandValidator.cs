using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Domain.Common;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Sponsors.Offers.Validators;

internal sealed class UpdateSponsorOfferCommandValidator : AbstractValidator<UpdateSponsorOfferCommand>
{
    public UpdateSponsorOfferCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
        
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.OfferIdRequired.Code]);

        RuleFor(x => x)
            .Must(x => x.DiscountAmount != null || x.DiscountPercentage != null)
            .WithMessage(errors[SponsorErrors.DiscountRequired.Code]);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercentage.HasValue)
            .WithMessage(errors[SponsorErrors.DiscountPercentageInvalid.Code]);

        RuleFor(x => x.MaxRedemptions)
            .GreaterThan(0)
            .When(x => x.MaxRedemptions.HasValue)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code]);
    }
}
