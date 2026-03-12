using Application.Common.Interfaces.Messaging;

namespace Application.Commands.Users.Tourists;

public record AddFavoriteCommand() : ICommand;

internal class AddFavoriteCommandHandler : ICommandHandler<AddFavoriteCommand>
{
    public Task<Result> HandleAsync(AddFavoriteCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
