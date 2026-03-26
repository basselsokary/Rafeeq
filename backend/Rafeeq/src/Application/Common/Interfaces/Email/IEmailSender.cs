namespace Application.Common.Interfaces.Email;

public interface IEmailService
{
    Task SendPasswordResetEmailAsync(
        string email, 
        string resetToken, 
        string userName,
        CancellationToken cancellationToken = default);

    Task SendEmailVerificationAsync(
        string email, 
        string verificationToken, 
        string userName,
        CancellationToken cancellationToken = default);

    Task SendWelcomeEmailAsync(
        string email, 
        string userName,
        CancellationToken cancellationToken = default);
}