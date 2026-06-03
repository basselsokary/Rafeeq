using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Services;
using Application.DTOs.Users;

namespace Application.Commands.Users.Admins;

public record DemoteToModeratorCommand(Guid UserId) : ICommand<UserDetailsDto>;

internal sealed class DemoteToModeratorCommandHandler(
    IUserManagementService userManagementService,
    IEmailService emailService) : ICommandHandler<DemoteToModeratorCommand, UserDetailsDto>
{
    public async Task<Result<UserDetailsDto>> HandleAsync(DemoteToModeratorCommand command, CancellationToken cancellationToken)
    {
        var result = await userManagementService.DemoteToModeratorAsync(command.UserId, cancellationToken);
        if (result.Failed)
            return result;

        var userDetails = result.Value;
        
        await emailService.SendAdminDemotionEmailAsync(userDetails.Email, userDetails.FirstName, cancellationToken);
        
        return userDetails;
    }
}
