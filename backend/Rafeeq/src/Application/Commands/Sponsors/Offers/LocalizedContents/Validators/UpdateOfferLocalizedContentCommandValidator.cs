using Application.Common.Interfaces.Localization;
using Domain.Entities.SponsorAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Sponsor;

namespace Application.Commands.Sponsors.Offers.LocalizedContents.Validators;

internal sealed class UpdateOfferLocalizedContentCommandValidator : AbstractValidator<UpdateOfferLocalizedContentCommand>
{
    public UpdateOfferLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.OfferIdRequired.Code]);

        RuleFor(x => x.ContentId)
            .NotEmpty()
            .WithMessage(errors["OFFER_CONTENT_ID_REQUIRED"]);

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
    }
}
