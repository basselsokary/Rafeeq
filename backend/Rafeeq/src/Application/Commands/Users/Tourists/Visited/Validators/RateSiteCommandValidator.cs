using Application.Common.Interfaces.Localization;
using Domain.Entities.TouristAggregate;
using Domain.ValueObjects;
using FluentValidation;

namespace Application.Commands.Users.Tourists.Visited.Validators;

internal sealed class RateSiteCommandValidator : AbstractValidator<RateSiteCommand>
{
    public RateSiteCommandValidator(IErrorLocalizer errorLocalizer)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errorLocalizer[TouristErrors.RequiredSiteId.Code]);

        RuleFor(x => x.Rating)
            .InclusiveBetween(Rating.Min, Rating.Max)
            .WithMessage(errorLocalizer[RatingErrors.OutOfRange(Rating.Min, Rating.Max).Code]);
    }
}