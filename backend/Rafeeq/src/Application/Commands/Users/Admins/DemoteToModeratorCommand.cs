using Application.Common.Interfaces.Services;
using Application.DTOs.Users;

namespace Application.Commands.Users.Admins;

public record DemoteToModeratorCommand(Guid UserId) : ICommand<UserDetailsDto>;

internal sealed class DemoteToModeratorCommandHandler(IUserManagementService userManagementService)
    : ICommandHandler<DemoteToModeratorCommand, UserDetailsDto>
{
    public async Task<Result<UserDetailsDto>> HandleAsync(DemoteToModeratorCommand command, CancellationToken cancellationToken)
    {
        return await userManagementService.DemoteToModeratorAsync(command.UserId, cancellationToken);
    }
}
