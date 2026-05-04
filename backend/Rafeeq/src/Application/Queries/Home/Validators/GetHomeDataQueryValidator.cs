using FluentValidation;

namespace Application.Queries.Home;

internal sealed class GetHomeDataQueryValidator : AbstractValidator<GetHomeDataQuery>
{
    public GetHomeDataQueryValidator()
    {
        RuleFor(q => q.Latitude)
            .InclusiveBetween(-90, 90)
            .When(q => q.Latitude.HasValue)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(q => q.Longitude)
            .InclusiveBetween(-180, 180)
            .When(q => q.Longitude.HasValue)
            .WithMessage("Longitude must be between -180 and 180.");
    }
}
