using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Users.Tourists.Favorites;

public sealed record RemoveFavoriteCommand(
    Guid SiteId) : ICommand;

internal sealed class RemoveFavoriteCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<RemoveFavoriteCommand>
{
    public async Task<Result> HandleAsync(RemoveFavoriteCommand command, CancellationToken cancellationToken)
    {
        var tourist = await unitOfWork.Tourists.GetWithFavouritesAsync(userContext.Id, cancellationToken);
        if (tourist == null)
            return TouristErrors.NotFound(userContext.Id);

        Result result = tourist.RemoveFavorite(command.SiteId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
