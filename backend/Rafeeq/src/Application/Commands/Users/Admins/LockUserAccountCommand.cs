using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record LockUserAccountCommand(
    Guid UserId,
    string Reason,
    DateTime? LockUntil) : ICommand;

internal sealed class LockUserAccountCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<LockUserAccountCommand>
{
    public async Task<Result> HandleAsync(LockUserAccountCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.LockUserAccountAsync(command.UserId, command.Reason, command.LockUntil, cancellationToken);
    }
}
