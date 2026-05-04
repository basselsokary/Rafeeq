using Application.Common.Interfaces.Localization;
using Domain.Entities.TouristAggregate;
using FluentValidation;

namespace Application.Commands.Users.Tourists.Favorites.Validators;

internal sealed class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand>
{
    public RemoveFavoriteCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[TouristErrors.RequiredSiteId.Code]);
    }
}
