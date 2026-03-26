using Application.Commands.Sponsors.Offers;
using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Validators;

namespace Application.Commands.Sponsors.Validators;

internal class AddSponsorOfferCommandValidator : AbstractValidator<AddSponsorOfferCommand>
{
    public AddSponsorOfferCommandValidator()
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(SponsorErrors.TitleRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SponsorErrors.DescriptionRequired.Message);

        RuleFor(x => x)
            .Must(x => x.DiscountAmount != null || x.DiscountPercentage != null)
            .WithMessage(SponsorErrors.DiscountRequired.Message);
        
        RuleFor(x => x.DiscountAmount)
            .SetValidator(new MoneyValidator()!)
            .When(x => x.DiscountAmount is not null);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercentage.HasValue)
            .WithMessage(SponsorErrors.DiscountPercentageInvalid.Message);

        RuleFor(x => x.ValidityPeriod)
            .NotNull()
            .SetValidator(new DateRangeValidator());

        RuleFor(x => x.MaxRedemptions)
            .GreaterThan(0)
            .When(x => x.MaxRedemptions.HasValue);

        RuleFor(x => x.TermsAndConditions)
            .NotEmpty()
            .When(x => x.TermsAndConditions != null);
    }
}

internal class RemoveSponsorOfferCommandValidator : AbstractValidator<RemoveSponsorOfferCommand>
{
    public RemoveSponsorOfferCommandValidator()
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(SponsorErrors.OfferIdRequired.Message);
    }
    
}

internal class UpdateSponsorOfferCommandValidator : AbstractValidator<UpdateSponsorOfferCommand>
{
    public UpdateSponsorOfferCommandValidator()
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(SponsorErrors.OfferIdRequired.Message);
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(SponsorErrors.TitleRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SponsorErrors.DescriptionRequired.Message);

        RuleFor(x => x)
            .Must(x => x.DiscountAmount != null || x.DiscountPercentage != null)
            .WithMessage(SponsorErrors.DiscountRequired.Message);

        RuleFor(x => x.DiscountAmount)
            .SetValidator(new MoneyValidator()!)
            .When(x => x.DiscountAmount is not null);

        RuleFor(x => x.DiscountPercentage)
            .InclusiveBetween(0, 100)
            .When(x => x.DiscountPercentage.HasValue)
            .WithMessage(SponsorErrors.DiscountPercentageInvalid.Message);
        
        RuleFor(x => x.ValidityPeriod)
            .NotNull()
            .SetValidator(new DateRangeValidator());

        RuleFor(x => x.MaxRedemptions)
            .GreaterThan(0)
            .When(x => x.MaxRedemptions.HasValue);

        RuleFor(x => x.TermsAndConditions)
            .NotEmpty()
            .When(x => x.TermsAndConditions != null);
    }
}

internal class RedeemSponsorOfferCommandValidator : AbstractValidator<RedeemSponsorOfferCommand>
{
    public RedeemSponsorOfferCommandValidator()
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(SponsorErrors.OfferIdRequired.Message);
    }
}

internal class SetSponsorOfferPromoCodeCommandValidator : AbstractValidator<SetSponsorOfferPromoCodeCommand>
{
    public SetSponsorOfferPromoCodeCommandValidator()
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
        
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(SponsorErrors.OfferIdRequired.Message);
        
        RuleFor(x => x.PromoCode)
            .NotEmpty()
            .WithMessage(SponsorErrors.PromoCodeRequired.Message);
    }
}

internal class ActivateSponsorOfferCommandValidator : AbstractValidator<ActivateSponsorOfferCommand>
{
    public ActivateSponsorOfferCommandValidator()
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);

        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(SponsorErrors.OfferIdRequired.Message);
    }
}
