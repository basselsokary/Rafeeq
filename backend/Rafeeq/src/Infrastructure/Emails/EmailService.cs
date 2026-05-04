using Application.Common.Interfaces.Email;
using Application.DTOs.Email;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Emails;

internal class EmailService(
    IOptions<EmailSettings> settings,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailSettings _settings = settings.Value;

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetToken,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var resetLink = $"{_settings.ApplicationUrl}/reset-password?token={resetToken}";
        var template = EmailTemplates.PasswordReset(userName, resetLink);
        
        await SendEmailAsync(email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
        
        logger.LogInformation("Password reset email sent to {Email}", email);
    }

    public async Task SendEmailVerificationAsync(
        string email,
        string verificationToken,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var verificationLink = $"{_settings.ApplicationUrl}/verify-email?token={verificationToken}";
        var template = EmailTemplates.EmailVerification(userName, verificationLink);
        
        await SendEmailAsync(email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
        
        logger.LogInformation("Email verification sent to {Email}", email);
    }

    public async Task SendWelcomeEmailAsync(
        string email,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.WelcomeTourist(userName);
        await SendEmailAsync(email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
        
        logger.LogInformation("Welcome email sent to new user {Email}", email);
    }

    public async Task SendWelcomeModeratorAsync(
        string firstName,
        string email,
        string tempPassword,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.WelcomeModerator(firstName, email, tempPassword);
        await SendEmailAsync(email, firstName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);

        logger.LogInformation("Welcome email sent to new moderator {Email}", email);
    }

    public async Task SendTripReminderEmailAsync(
        string email,
        string userName,
        string tripName,
        DateTime tripDate,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.TripReminder(userName, tripName, tripDate);
        
        await SendEmailAsync(
            email,
            userName,
            template.Subject,
            template.HtmlBody,
            template.TextBody,
            cancellationToken);
        
        logger.LogInformation("Trip reminder sent to {Email} for trip {TripName}", email, tripName);
    }

    public async Task SendReviewResponseNotificationAsync(
        string email,
        string userName,
        string siteName,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.ReviewResponse(userName, siteName);
        
        await SendEmailAsync(
            email,
            userName,
            template.Subject,
            template.HtmlBody,
            template.TextBody,
            cancellationToken);
        
        logger.LogInformation("Review response notification sent to {Email}", email);
    }

    private async Task SendEmailAsync(
        string toEmail,
        string toName,
        string subject,
        string htmlBody,
        string textBody,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new MimeMessage();
            
            message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            
            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            };
            message.Body = builder.ToMessageBody();

            // Send using SMTP
            using var client = new SmtpClient();
            
            // For debugging/development, you can log protocol
            // client.ServerCertificateValidationCallback = (s,c,h,e) => true;
            
            await client.ConnectAsync(
                _settings.SmtpHost,
                _settings.SmtpPort,
                false, // For now
                cancellationToken);

            // Authenticate (if credentials provided)
            if (!string.IsNullOrEmpty(_settings.SmtpUsername))
            {
                await client.AuthenticateAsync(
                    _settings.SmtpUsername,
                    _settings.SmtpPassword,
                    cancellationToken);
            }

            await client.SendAsync(message, cancellationToken);
            
            await client.DisconnectAsync(true, cancellationToken);
            
            logger.LogInformation("Email '{Subject}' sent successfully to {Email}", subject, toEmail);
        }
        catch (AuthenticationException ex)
        {
            logger.LogError(ex, "Authentication failed while sending email to {Email}. Check SMTP credentials.", toEmail);
            throw new InvalidOperationException($"Failed to authenticate with email server for {toEmail}", ex);
        }
        catch (SmtpCommandException ex)
        {
            logger.LogError(ex, "SMTP command error while sending email to {Email}: {Message}", toEmail, ex.Message);
            throw new InvalidOperationException($"SMTP error while sending email to {toEmail}: {ex.Message}", ex);
        }
        catch (SmtpProtocolException ex)
        {
            logger.LogError(ex, "SMTP protocol error while sending email to {Email}: {Message}", toEmail, ex.Message);
            throw new InvalidOperationException($"SMTP protocol error while sending email to {toEmail}: {ex.Message}", ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while sending email to {Email}: {Message}", toEmail, ex.Message);
            throw new InvalidOperationException($"Failed to send email to {toEmail}", ex);
        }
    }
}
