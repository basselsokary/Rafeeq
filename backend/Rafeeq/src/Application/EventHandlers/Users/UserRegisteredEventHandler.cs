using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers.Users;

internal class UserRegisteredEventHandler(
    IEmailService emailService,
    IIdentityService identityService,
    ILogger<UserRegisteredEventHandler> logger) : IDomainEventHandler<UserRegisteredEvent>
{
    public async Task HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Result<(string ResetToken, string UserName)> result = await identityService.GenerateEmailConfirmationTokenAsync(domainEvent.Email);
        if (result.Failed)
        {
            logger.LogError(
                "Failed to generate email confirmation token for user {Email}: {Error}",
                domainEvent.Email,
                result.Error.Message);

            return;
        }

        await emailService.SendEmailVerificationAsync(
            domainEvent.Email,
            result.Value.ResetToken,
            domainEvent.UserName,
            cancellationToken);
    }
}
