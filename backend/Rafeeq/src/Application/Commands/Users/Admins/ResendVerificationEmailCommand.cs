using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Services;

namespace Application.Commands.Users.Admins;

public record ResendVerificationEmailCommand(Guid UserId) : ICommand<EmailSentResultDto>;

public record EmailSentResultDto(
    string Email,
    string UserName,
    string Token);

internal sealed class ResendVerificationEmailCommandHandler(
    IUserManagementService userManagementService,
    IEmailService emailService) : ICommandHandler<ResendVerificationEmailCommand, EmailSentResultDto>
{
    public async Task<Result<EmailSentResultDto>> HandleAsync(ResendVerificationEmailCommand command, CancellationToken cancellationToken)
    {
        var result = await userManagementService.ResendVerificationEmailAsync(command.UserId, cancellationToken);
        if (!result.Succeeded)
            return Result.Failure<EmailSentResultDto>(result.Error);

        await emailService.SendEmailVerificationAsync(
            result.Value.Email,
            result.Value.Token,
            result.Value.UserName,
            cancellationToken);

        return Result.Success(result.Value);
    }
}
