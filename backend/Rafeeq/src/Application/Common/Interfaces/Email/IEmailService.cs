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

    Task SendWelcomeModeratorAsync(
        string firstName,
        string email,
        string tempPassword,
        CancellationToken cancellationToken = default);

    Task SendTripReminderEmailAsync(
        string email,
        string userName,
        string tripName,
        DateTime tripDate,
        CancellationToken cancellationToken = default);

    Task SendReviewResponseNotificationAsync(
        string email,
        string userName,
        string siteName,
        CancellationToken cancellationToken = default);
}