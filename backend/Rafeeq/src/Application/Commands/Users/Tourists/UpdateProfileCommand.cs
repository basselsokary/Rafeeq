using Application.Common.Interfaces.Messaging;

namespace Application.Commands.Users.Tourists;

public record UpdateProfileCommand() : ICommand;

internal class UpdateProfileCommandHandler : ICommandHandler<UpdateProfileCommand>
{
    public Task<Result> HandleAsync(UpdateProfileCommand command, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
