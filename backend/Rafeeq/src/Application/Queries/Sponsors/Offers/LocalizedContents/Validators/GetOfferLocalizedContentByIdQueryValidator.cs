using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sponsors.Offers.LocalizedContents.Validators;

internal sealed class GetOfferLocalizedContentByIdQueryValidator : AbstractValidator<GetOfferLocalizedContentByIdQuery>
{
    public GetOfferLocalizedContentByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.OfferIdRequired.Code]);

        RuleFor(x => x.ContentId)
            .NotEmpty()
            .WithMessage(errors["OFFER_CONTENT_ID_REQUIRED"]);
    }
}
