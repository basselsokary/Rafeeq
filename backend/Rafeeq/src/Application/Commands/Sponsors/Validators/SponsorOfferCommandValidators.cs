using Application.Commands.Sponsors.Offers;
using Domain.Entities.SponsorAggregate;
using FluentValidation;

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
        
        RuleFor(x => x.ValidityPeriod)
            .NotNull();
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
        
        RuleFor(x => x.ValidityPeriod)
            .NotNull();
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
