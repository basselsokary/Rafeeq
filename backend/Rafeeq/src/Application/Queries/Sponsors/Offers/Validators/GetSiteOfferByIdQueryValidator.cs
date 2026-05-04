using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sponsors.Offers.Validators;

internal sealed class GetSiteOfferByIdQueryValidator : AbstractValidator<GetSponsorOfferByIdQuery>
{
    public GetSiteOfferByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.OfferIdRequired.Code]);
    }
}
