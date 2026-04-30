using Domain.Entities.SponsorAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sponsors.Offers.LocalizedContents.Validators;

internal sealed class GetOfferLocalizedContentsQueryValidator : AbstractValidator<GetOfferLocalizedContentsQuery>
{
    public GetOfferLocalizedContentsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.OfferIdRequired.Code]);
    }
}
