using Application.Queries.Common.Validators;
using Domain.Entities.SponsorAggregate;
using FluentValidation;

namespace Application.Queries.Sponsors.Offers.Validators;

internal class GetSiteOffersQueryValidator : AbstractValidator<GetSiteOffersQuery>
{
    public GetSiteOffersQueryValidator()
    {
        RuleFor(x => x.SponsorId)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
    }
}

internal class GetAllSiteOffersAsyncValidator : AbstractValidator<GetAllSiteOffersAsync>
{
    public GetAllSiteOffersAsyncValidator()
    {
        RuleFor(x => x.Filters)
            .NotNull()
            .SetValidator(new SponsorFiltersValidator());

        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());
    }
}

internal class GetSiteOfferByIdAsyncValidator : AbstractValidator<GetSiteOfferByIdAsync>
{
    public GetSiteOfferByIdAsyncValidator()
    {
        RuleFor(x => x.OfferId)
            .NotEmpty()
            .WithMessage(SponsorErrors.OfferIdRequired.Message);
    }
}
