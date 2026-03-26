using Application.Queries.Common.Validators;
using Domain.Entities.SponsorAggregate;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Queries.Sponsors.Validators;

internal class GetSponsorsQueryValidator : AbstractValidator<GetSponsorsQuery>
{
    public GetSponsorsQueryValidator()
    {
        RuleFor(x => x.Filters)
            .NotNull()
            .SetValidator(new SponsorFiltersValidator());

        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());

        RuleFor(x => x.SearchTerm)
            .Must(searchTerm => searchTerm == null || !string.IsNullOrWhiteSpace(searchTerm))
            .WithMessage("Search term cannot be whitespace.");
    }
}

internal class GetNearbySponsorsQueryValidator : AbstractValidator<GetNearbySponsorsQuery>
{
    public GetNearbySponsorsQueryValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude);

        RuleFor(x => x.Filters)
            .NotNull()
            .SetValidator(new SponsorFiltersValidator());

        RuleFor(x => x.Count)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0)
            .LessThanOrEqualTo(20);
    }
}

internal class GetSponsorByIdQueryValidator : AbstractValidator<GetSponsorByIdQuery>
{
    public GetSponsorByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
    }
}

internal class GetSponsorByIdForAdminQueryValidator : AbstractValidator<GetSponsorByIdForAdminQuery>
{
    public GetSponsorByIdForAdminQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SponsorErrors.IdRequired.Message);
    }
}
