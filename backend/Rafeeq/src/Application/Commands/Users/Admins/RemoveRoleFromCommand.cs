using Application.Common.Interfaces.Services;
using Domain.Enums;

namespace Application.Commands.Users.Admins;

public sealed record RemoveRoleFromCommand(
    Guid UserId,
    UserRole Role) : ICommand;

internal sealed class RemoveRoleFromCommandHandler(
    IAdminService adminService) : ICommandHandler<RemoveRoleFromCommand>
{
    public async Task<Result> HandleAsync(RemoveRoleFromCommand command, CancellationToken cancellationToken)
    {
        return await adminService.RemoveRoleAsync(command.UserId, command.Role);
    }
}
