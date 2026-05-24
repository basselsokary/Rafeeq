using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record ConfirmUserEmailCommand(Guid UserId) : ICommand;

internal sealed class ConfirmUserEmailCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<ConfirmUserEmailCommand>
{
    public async Task<Result> HandleAsync(ConfirmUserEmailCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.ConfirmUserEmailAsync(command.UserId, cancellationToken);
    }
}
