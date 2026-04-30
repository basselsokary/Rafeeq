using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sponsors.Offers.Validators;

internal sealed class GetSiteOffersQueryValidator : AbstractValidator<GetSponsorOffersByIdQuery>
{
    public GetSiteOffersQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
    }
}
