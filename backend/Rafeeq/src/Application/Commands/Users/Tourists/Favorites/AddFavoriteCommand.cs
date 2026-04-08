using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Users.Tourists.Favorites;

public record AddFavoriteCommand(
    Guid SiteId) : ICommand;

internal class AddFavoriteCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<AddFavoriteCommand>
{
    public async Task<Result> HandleAsync(AddFavoriteCommand command, CancellationToken cancellationToken)
    {
        var tourist = await unitOfWork.Tourists.GetWithFavouritesAsync(userContext.Id, cancellationToken);
        if (tourist == null)
            return TouristErrors.NotFound(userContext.Id);

        Result result = tourist.AddFavorite(command.SiteId);
        if (result.Failed)
            return result;

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
