using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record UnlockUserAccountCommand(Guid UserId) : ICommand;

internal sealed class UnlockUserAccountCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<UnlockUserAccountCommand>
{
    public async Task<Result> HandleAsync(UnlockUserAccountCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.UnlockUserAccountAsync(command.UserId, cancellationToken);
    }
}
