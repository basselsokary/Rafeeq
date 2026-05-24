using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record SuspendUserAccountCommand(
    Guid UserId,
    string Reason,
    DateTime SuspendUntil,
    bool NotifyUser = true) : ICommand;

internal sealed class SuspendUserAccountCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<SuspendUserAccountCommand>
{
    public async Task<Result> HandleAsync(SuspendUserAccountCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.SuspendUserAccountAsync(
            command.UserId,
            command.SuspendUntil,
            command.Reason,
            command.NotifyUser,
            cancellationToken);
    }
}
