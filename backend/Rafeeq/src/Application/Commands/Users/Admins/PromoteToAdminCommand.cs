using Application.Common.Interfaces.Services;
using Application.DTOs.Users;

namespace Application.Commands.Users.Admins;

public record PromoteToAdminCommand(Guid UserId) : ICommand<UserDetailsDto>;

internal sealed class PromoteToAdminCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<PromoteToAdminCommand, UserDetailsDto>
{
    public async Task<Result<UserDetailsDto>> HandleAsync(PromoteToAdminCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.PromoteToAdminAsync(command.UserId, cancellationToken);
    }
}
