using FluentValidation;

namespace Application.Queries.Home;

internal sealed class GetMapPlacesQueryValidator : AbstractValidator<GetMapPlacesQuery>
{
    public GetMapPlacesQueryValidator()
    {
        RuleFor(q => q.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(q => q.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180.");

        RuleFor(q => q.RadiusKm)
            .GreaterThan(0)
            .LessThanOrEqualTo(100)
            .WithMessage("RadiusKm must be greater than 0 and less than or equal to 100.");
    }
}