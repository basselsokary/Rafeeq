using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Services;
using Application.DTOs.Users;

namespace Application.Commands.Users.Admins;

public record PromoteToAdminCommand(Guid UserId) : ICommand<UserDetailsDto>;

internal sealed class PromoteToAdminCommandHandler(
    IUserManagementService userManagementService,
    IEmailService emailService): ICommandHandler<PromoteToAdminCommand, UserDetailsDto>
{
    public async Task<Result<UserDetailsDto>> HandleAsync(PromoteToAdminCommand command, CancellationToken cancellationToken)
    {
        var result = await userManagementService.PromoteToAdminAsync(command.UserId, cancellationToken);
        if (result.Failed)
            return result;

        var userDetails = result.Value;
        
        await emailService.SendAdminPromotionEmailAsync(userDetails.Email, userDetails.FirstName, cancellationToken);
        
        return userDetails;
    }
}
