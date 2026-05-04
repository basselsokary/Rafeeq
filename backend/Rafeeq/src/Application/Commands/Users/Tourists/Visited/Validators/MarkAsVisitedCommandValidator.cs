using Application.Common.Interfaces.Localization;
using Domain.Entities.TouristAggregate;
using FluentValidation;

namespace Application.Commands.Users.Tourists.Visited.Validators;

internal sealed class MarkAsVisitedCommandValidator : AbstractValidator<MarkAsVisitedCommand>
{
    public MarkAsVisitedCommandValidator(IErrorLocalizer errorLocalizer)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errorLocalizer[TouristErrors.RequiredSiteId.Code]);

        RuleFor(x => x.DurationMinutes)
            .GreaterThan(0)
            .WithMessage(errorLocalizer[TouristErrors.DurationInvalid.Code]);

        RuleFor(x => x.VisitedAt)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage(errorLocalizer[TouristErrors.VisitDateInFuture.Code]);
    }
}
