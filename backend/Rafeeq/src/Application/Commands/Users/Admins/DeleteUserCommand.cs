using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record DeleteUserCommand(
    Guid UserId,
    string Reason,
    bool NotifyUser = true) : ICommand;

internal sealed class DeleteUserCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> HandleAsync(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.DeleteUserAsync(command.UserId, command.Reason, command.NotifyUser, cancellationToken);
    }
}
