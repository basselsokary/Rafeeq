using Application.Commands.Users.Tourists.Favorites;
using FluentValidation;

namespace Application.Commands.Users.Tourists.Validators;

internal class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommand>
{
    public AddFavoriteCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty();
    }
}

internal class RemoveFavoriteCommandValidator : AbstractValidator<RemoveFavoriteCommand>
{
    public RemoveFavoriteCommandValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty();
    }
}
