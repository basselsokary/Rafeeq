using Application.Common.Interfaces.Messaging;

namespace Application.Commands.Users.Tourists;

public record DeleteFavoriteCommand() : ICommand;

internal class DeleteFavoriteCommandHandler : ICommandHandler<DeleteFavoriteCommand>
{
    public Task<Result> HandleAsync(DeleteFavoriteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
