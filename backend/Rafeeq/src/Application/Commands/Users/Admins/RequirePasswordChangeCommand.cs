using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record RequirePasswordChangeCommand(Guid UserId) : ICommand;

internal sealed class RequirePasswordChangeCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<RequirePasswordChangeCommand>
{
    public async Task<Result> HandleAsync(RequirePasswordChangeCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.RequirePasswordChangeAsync(command.UserId, cancellationToken);
    }
}
