using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Domain.Common;
using Application.Common.Interfaces.Localization;
using static Domain.Common.Constants.DomainConstants.Sponsor;

namespace Application.Commands.Sponsors.Offers.Validators;

internal sealed class AddSponsorOfferCommandValidator : AbstractValidator<AddSponsorOfferCommand>
{
    public AddSponsorOfferCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);

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

internal sealed class AddOfferLocalizedContentDtoValidator : AbstractValidator<AddOfferLocalizedContentDto>
{
    public AddOfferLocalizedContentDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.TitleRequired.Code])
            .MaximumLength(MaxTitleLength)
            .WithMessage(errors.Format(SponsorErrors.ExceededTitleLength.Code, MaxTitleLength));
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(SponsorErrors.ExceededDescriptionLength.Code, MaxDescriptionLength));

        RuleFor(x => x.TermsAndConditions)
            .NotEmpty()
            .When(x => x.TermsAndConditions != null)
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
    }
}