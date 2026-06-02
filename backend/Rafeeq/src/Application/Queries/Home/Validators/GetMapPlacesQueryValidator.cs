using Domain.Common;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Queries.Home.Validators;

internal sealed class GetMapPlacesQueryValidator : AbstractValidator<GetMapPlacesQuery>
{
    public GetMapPlacesQueryValidator()
    {
        RuleFor(q => q.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage(q => GeoLocationErrors.InvalidLatitude(q.Latitude).Code);

        RuleFor(q => q.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage(q => GeoLocationErrors.InvalidLongitude(q.Longitude).Code);

        RuleFor(q => q.RadiusKm)
            .GreaterThanOrEqualTo(10)
            .LessThanOrEqualTo(100)
            .WithMessage(ValidationErrors.RangeInvalid(10.ToString(), 100.ToString()).Code);
        
        RuleFor(q => q.Count)
            .GreaterThanOrEqualTo(10)
            .LessThanOrEqualTo(50)
            .WithMessage(ValidationErrors.RangeInvalid(10.ToString(), 50.ToString()).Code);
    }
}