using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record ResetUserPasswordCommand(Guid UserId) : ICommand<PasswordResetResultDto>;

public record PasswordResetResultDto(
    string Email,
    string UserName,
    string Token);

internal sealed class ResetUserPasswordCommandHandler(
    IUserManagementService userManagementService,
    IEmailService emailService) : ICommandHandler<ResetUserPasswordCommand, PasswordResetResultDto>
{
    public async Task<Result<PasswordResetResultDto>> HandleAsync(ResetUserPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await userManagementService.ResetUserPasswordAsync(command.UserId, cancellationToken);
        if (result.Failed)
            return result.Error;
        
        PasswordResetResultDto dto = result.Value;
        await emailService.SendPasswordResetEmailAsync(dto.Email, dto.Token, dto.UserName, cancellationToken);
        
        return result;
    }
}
