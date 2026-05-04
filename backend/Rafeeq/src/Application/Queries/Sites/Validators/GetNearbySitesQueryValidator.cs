using FluentValidation;
using Application.Queries.Common.Validators;
using Domain.ValueObjects;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Sites.Validators;

internal sealed class GetNearbySitesQueryValidator : AbstractValidator<GetNearbySitesQuery>
{
    public GetNearbySitesQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLatitude(x.Latitude).Code, x.Latitude));

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLongitude(x.Longitude).Code, x.Longitude));

        RuleFor(x => x.Filters)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new SiteFiltersValidator(errors));

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code])
            .LessThanOrEqualTo(20)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);
    }
}
