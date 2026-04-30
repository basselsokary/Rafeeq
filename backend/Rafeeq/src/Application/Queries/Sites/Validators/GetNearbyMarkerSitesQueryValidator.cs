using FluentValidation;
using Domain.ValueObjects;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sites.Validators;

internal sealed class GetNearbyMarkerSitesQueryValidator : AbstractValidator<GetNearbyMarkerSitesQuery>
{
    public GetNearbyMarkerSitesQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Latitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLatitude(x.Latitude).Code, x.Latitude));

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLongitude(x.Longitude).Code, x.Longitude));
    }
}

