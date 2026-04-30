using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;

namespace Application.Commands.Users;

public sealed record ResendEmailVerificationCommand(
    string Email) : ICommand;

internal class ResendEmailVerificationCommandHandler(
    IIdentityService identityService,
    IEmailService emailService) : ICommandHandler<ResendEmailVerificationCommand>
{
    public async Task<Result> HandleAsync(ResendEmailVerificationCommand command, CancellationToken cancellationToken = default)
    {   
        var result = await identityService.GenerateEmailConfirmationTokenAsync(command.Email);
        if (result.Failed)
            return result;
        
        await emailService.SendEmailVerificationAsync(
            command.Email,
            result.Value.ResetToken,
            result.Value.UserName,
            cancellationToken);

        return Result.Success();
    }
}