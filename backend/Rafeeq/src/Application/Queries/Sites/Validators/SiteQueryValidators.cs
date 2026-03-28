using FluentValidation;
using Application.Queries.Common.Validators;
using Domain.Entities.SiteAggregate;
using Domain.ValueObjects;

namespace Application.Queries.Sites.Validators;

internal class GetNearbySitesQueryValidator : AbstractValidator<GetNearbySitesQuery>
{
    public GetNearbySitesQueryValidator()
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude);

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude);

        RuleFor(x => x.Filters)
            .NotNull()
            .SetValidator(new SiteFiltersValidator());

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0)
            .LessThanOrEqualTo(20);
    }
}

internal class GetSiteByIdQueryValidator : AbstractValidator<GetSiteByIdQuery>
{
    public GetSiteByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
    }
}

internal class GetSiteByIdForAdminQueryValidator : AbstractValidator<GetSiteByIdForAdminQuery>
{
    public GetSiteByIdForAdminQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
    }
}

internal class GetSitesWithinBoundsQueryValidator : AbstractValidator<GetSitesWithinBoundsQuery>
{
    public GetSitesWithinBoundsQueryValidator()
    {
        RuleFor(x => x.Bounds)
            .NotNull()
            .SetValidator(new BoundingBoxValidator());

        RuleFor(x => x.Filters)
            .NotNull()
            .SetValidator(new SiteFiltersValidator());
    }
}

internal class GetFeaturedSitesQueryValidator : AbstractValidator<GetFeaturedSitesQuery>
{
    public GetFeaturedSitesQueryValidator()
    {
        RuleFor(x => x.City)
            .NotEmpty()
            .When(x => x.City.HasValue);
    }
}

internal class GetSitesQueryValidator : AbstractValidator<GetSitesQuery>
{
    public GetSitesQueryValidator()
    {
        // RuleFor(x => x.SearchTerm)
        //     .Must(searchTerm => !string.IsNullOrWhiteSpace(searchTerm))
        //     .When(searchTerm => searchTerm != null)
        //     .WithMessage("Search term cannot be whitespace.");

        RuleFor(x => x.Filters)
            .SetValidator(new SiteFiltersValidator())
            .When(x => x.Filters is not null);

        RuleFor(x => x.Paging)
            .SetValidator(new PagingParametersValidator())
            .When(x => x.Paging is not null);
    }
}

