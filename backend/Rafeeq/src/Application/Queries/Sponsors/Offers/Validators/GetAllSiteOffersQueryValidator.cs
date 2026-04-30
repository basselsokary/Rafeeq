using Application.Common.Interfaces.Localization;
using Domain.Entities.SponsorAggregate;
using FluentValidation;

namespace Application.Queries.Sponsors.Offers.Validators;

internal sealed class GetSponsorOffersByIdQueryValidator : AbstractValidator<GetSponsorOffersByIdQuery>
{
    public GetSponsorOffersByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(errors[SponsorErrors.IdRequired.Code]);
    }
}
