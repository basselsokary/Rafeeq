namespace Application.Commands.Users.Tourists.Favorites;

public record DeleteFavoriteCommand() : ICommand;

internal class DeleteFavoriteCommandHandler : ICommandHandler<DeleteFavoriteCommand>
{
    public Task<Result> HandleAsync(DeleteFavoriteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
