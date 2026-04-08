using Application.DTOs.Common;
using Application.DTOs.Sites;
using Application.DTOs.Sponsors;
using Domain.ValueObjects;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Review;

namespace Application.Queries.Common.Validators;

internal class PagingParametersValidator : AbstractValidator<PagingParameters>
{
    public PagingParametersValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(100);
    }
}

internal class SiteFiltersValidator : AbstractValidator<SiteFilters>
{
    public SiteFiltersValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue);

        RuleFor(x => x.City)
            .NotEmpty()
            .When(x => x.City.HasValue);

        RuleFor(x => x.MinRating)
            .InclusiveBetween(MinRatingValue, MaxRatingValue)
            .When(x => x.MinRating.HasValue);

        RuleFor(x => x.MaxRating)
            .InclusiveBetween(MinRatingValue, MaxRatingValue)
            .When(x => x.MaxRating.HasValue);

        RuleFor(x => x)
            .Must(filters => !filters.MinRating.HasValue || !filters.MaxRating.HasValue || filters.MinRating <= filters.MaxRating)
            .WithMessage(RatingErrors.OutOfRange(MinRatingValue, MaxRatingValue).Message);
    }
}

internal class BoundingBoxValidator : AbstractValidator<BoundingBox>
{
    public BoundingBoxValidator()
    {
        RuleFor(x => x.NorthLatitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude);

        RuleFor(x => x.SouthLatitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude);

        RuleFor(x => x.EastLongitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude);

        RuleFor(x => x.WestLongitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude);

        RuleFor(x => x)
            .Must(bounds => bounds.NorthLatitude >= bounds.SouthLatitude)
            .WithMessage("NorthLatitude must be greater than or equal to SouthLatitude.");
    }
}

internal class SponsorFiltersValidator : AbstractValidator<SponsorFilters>
{
    public SponsorFiltersValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type.HasValue);

        RuleFor(x => x.Tier)
            .IsInEnum()
            .When(x => x.Tier.HasValue);
    }
}
