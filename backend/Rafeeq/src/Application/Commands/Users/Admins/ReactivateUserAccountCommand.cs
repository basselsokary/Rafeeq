using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record ReactivateUserAccountCommand(Guid UserId) : ICommand;

internal sealed class ReactivateUserAccountCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<ReactivateUserAccountCommand>
{
    public async Task<Result> HandleAsync(ReactivateUserAccountCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.ReactivateUserAccountAsync(command.UserId, cancellationToken);
    }
}
