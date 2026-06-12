using Application.Queries.Common.Validators;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Sponsors.Validators;

internal sealed class GetNearbySponsorsQueryValidator : AbstractValidator<GetNearbySponsorsQuery>
{
    public GetNearbySponsorsQueryValidator(IErrorLocalizer errors)
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
            .SetValidator(new SponsorFiltersValidator(errors));

        RuleFor(x => x.Count)
            .GreaterThan(0)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code])
            .LessThanOrEqualTo(100)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);

        RuleFor(x => x.RadiusKm)
            .GreaterThan(0)
            .WithMessage(errors[ValidationErrors.NumberMustBeGreaterThanZero.Code])
            .LessThanOrEqualTo(40)
            .WithMessage(errors[ValidationErrors.MaximumLengthExceeded.Code]);
    }
}
