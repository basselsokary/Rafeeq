using Application.DTOs.Common;
using Domain.ValueObjects;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Common.Validators;

internal sealed class BoundingBoxValidator : AbstractValidator<BoundingBox>
{
    public BoundingBoxValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.NorthLatitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLatitude(x.NorthLatitude).Code, x.NorthLatitude));

        RuleFor(x => x.SouthLatitude)
            .InclusiveBetween(-GeoLocation.BoundLatitude, GeoLocation.BoundLatitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLatitude(x.SouthLatitude).Code, x.SouthLatitude));

        RuleFor(x => x.EastLongitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLongitude(x.EastLongitude).Code, x.EastLongitude));

        RuleFor(x => x.WestLongitude)
            .InclusiveBetween(-GeoLocation.BoundLongitude, GeoLocation.BoundLongitude)
            .WithMessage(x => errors.Format(GeoLocationErrors.InvalidLongitude(x.WestLongitude).Code, x.WestLongitude));

        RuleFor(x => x)
            .Must(bounds => bounds.NorthLatitude >= bounds.SouthLatitude)
            .WithMessage(bounds => errors[ValidationErrors.RangeInvalid(bounds.SouthLatitude.ToString(), bounds.NorthLatitude.ToString()).Code]);
        
        RuleFor(x => x)
            .Must(bounds => bounds.EastLongitude >= bounds.WestLongitude)
            .WithMessage(bounds => errors[ValidationErrors.RangeInvalid(bounds.WestLongitude.ToString(), bounds.EastLongitude.ToString()).Code]);
    }
}
