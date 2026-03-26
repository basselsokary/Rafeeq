using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;

namespace Application.Commands.Users;

public record ForgotPasswordCommand(string Email) : ICommand;

public class ForgotPasswordCommandHandler(
    IIdentityService identityService,
    IEmailService emailService) : ICommandHandler<ForgotPasswordCommand>
{
    public async Task<Result> HandleAsync(ForgotPasswordCommand command, CancellationToken cancellationToken)
    {
        var result = await identityService.GeneratePasswordResetTokenAsync(command.Email);

        if (result.Failed)
            return result;
            
        await emailService.SendPasswordResetEmailAsync(
            command.Email,
            result.Value.ResetToken,
            result.Value.UserName,
            cancellationToken);

        return Result.Success();
    }
}