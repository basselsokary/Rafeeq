using Application.Common.Interfaces.Localization;
using Domain.Entities.TouristAggregate;
using FluentValidation;

namespace Application.Commands.Users.Tourists.Visited.Validators;

internal sealed class RemoveRatingCommandValidator : AbstractValidator<RemoveRatingCommand>
{
    public RemoveRatingCommandValidator(IErrorLocalizer errorLocalizer)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errorLocalizer[TouristErrors.RequiredSiteId.Code]);
    }
}
