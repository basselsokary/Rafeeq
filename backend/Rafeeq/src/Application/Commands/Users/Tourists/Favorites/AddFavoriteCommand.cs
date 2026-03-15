namespace Application.Commands.Users.Tourists.Favorites;

public record AddFavoriteCommand() : ICommand;

internal class AddFavoriteCommandHandler : ICommandHandler<AddFavoriteCommand>
{
    public Task<Result> HandleAsync(AddFavoriteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
