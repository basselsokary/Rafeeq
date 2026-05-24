using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record PermanentlyDeleteUserCommand(
    Guid UserId,
    string Reason,
    bool ConfirmDeletion) : ICommand;

internal sealed class PermanentlyDeleteUserCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<PermanentlyDeleteUserCommand>
{
    public async Task<Result> HandleAsync(PermanentlyDeleteUserCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.PermanentlyDeleteUserAsync(
            command.UserId,
            command.Reason,
            command.ConfirmDeletion,
            cancellationToken);
    }
}
