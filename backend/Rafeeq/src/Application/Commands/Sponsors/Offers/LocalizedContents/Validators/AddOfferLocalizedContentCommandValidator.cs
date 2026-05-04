using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.SponsorAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Sponsor;

namespace Application.Commands.Sponsors.Offers.LocalizedContents.Validators;

internal sealed class AddOfferLocalizedContentCommandValidator : AbstractValidator<AddOfferLocalizedContentsCommand>
{
    public AddOfferLocalizedContentCommandValidator(IErrorLocalizer errors)
    {

        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.OfferIdRequired.Code]);
        
        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddOfferLocalizedContentsDtoCommandValidator(errors));
    }
}

internal sealed class AddOfferLocalizedContentsDtoCommandValidator : AbstractValidator<AddOfferLocalizedContentsDtoCommand>
{
    public AddOfferLocalizedContentsDtoCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Language.ToString()));

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
