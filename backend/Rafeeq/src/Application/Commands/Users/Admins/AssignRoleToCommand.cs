using Application.Common.Interfaces.Services;
using Domain.Enums;

namespace Application.Commands.Users.Admins;

public sealed record AssignRoleToCommand(
    Guid UserId,
    UserRole Role) : ICommand;

internal sealed class AssignRoleToCommandHandler(
    IAdminService adminService) : ICommandHandler<AssignRoleToCommand>
{
    public async Task<Result> HandleAsync(AssignRoleToCommand command, CancellationToken cancellationToken)
    {
        return await adminService.AssignRoleAsync(command.UserId, command.Role);
    }
}
