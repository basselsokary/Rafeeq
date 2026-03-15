using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Moderators;

public record ActivateUserCommand(
    Guid UserId,
    bool Active) : ICommand;

public class ActivateUserCommandHandler(
    IModeratorService moderatorService) : ICommandHandler<ActivateUserCommand>
{
    public async Task<Result> HandleAsync(ActivateUserCommand command, CancellationToken cancellationToken)
    {
        var result = await moderatorService.ActivateUserAsync(
            command.UserId,
            command.Active);

        if (result.Failed)
            return result;

        return Result.Success();
    }
}