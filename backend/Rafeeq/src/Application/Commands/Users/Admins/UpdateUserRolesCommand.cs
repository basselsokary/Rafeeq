using Application.Common.Interfaces.Services;
using Application.DTOs.Users;

namespace Application.Commands.Users.Admins;

public record UpdateUserRolesCommand(
    Guid UserId,
    List<string> Roles,
    string? Reason) : ICommand<UserDetailsDto>;

internal sealed class UpdateUserRolesCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<UpdateUserRolesCommand, UserDetailsDto>
{
    public async Task<Result<UserDetailsDto>> HandleAsync(UpdateUserRolesCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.UpdateUserRolesAsync(command.UserId, command.Roles, command.Reason, cancellationToken);
    }
}
