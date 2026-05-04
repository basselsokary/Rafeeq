using Application.Common.Interfaces.Authentication;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Entities.TouristAggregate;

namespace Application.Commands.Users.Tourists.Favorites;

public sealed record AddFavoriteCommand(
    Guid SiteId) : ICommand;

internal sealed class AddFavoriteCommandHandler(
    IUnitOfWork unitOfWork,
    IUserContext userContext) : ICommandHandler<AddFavoriteCommand>
{
    public async Task<Result> HandleAsync(AddFavoriteCommand command, CancellationToken cancellationToken)
    {
        var siteExists = await unitOfWork.Sites.AnyAsync(command.SiteId, cancellationToken);
        if (!siteExists)
            return SiteErrors.NotFound(command.SiteId);

        var tourist = await unitOfWork.Tourists.GetWithFavouritesAsync(userContext.Id, cancellationToken);
        if (tourist == null)
            return TouristErrors.NotFound(userContext.Id);

        var result = tourist.AddFavorite(command.SiteId);
        if (result.Failed)
            return result;

        await unitOfWork.AddAsync(result.Value, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
