using Application.Common.Interfaces.Email;
using Application.DTOs.Integrations.Email;
using Infrastructure.Authentication;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Emails;

internal class EmailService(
    IOptions<EmailOptions> emailOptions,
    IOptions<JwtOptions> jwtOptions,
    ILogger<EmailService> logger) : IEmailService
{
    private readonly EmailOptions _options = emailOptions.Value;
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public async Task SendPasswordResetEmailAsync(
        string email,
        string resetToken,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var resetLink = $"{_options.ApplicationUrl}/reset-password?token={resetToken}";
        var template = EmailTemplates.PasswordReset(userName, resetLink, _jwtOptions.TokenLifespanHours);
        
        await SendEmailAsync(
            email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
    }

    public async Task SendEmailVerificationAsync(
        string email,
        string verificationToken,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var verificationLink = $"{_options.ApplicationUrl}/verify-email?token={verificationToken}";
        var template = EmailTemplates.EmailVerification(userName, verificationLink, _jwtOptions.TokenLifespanHours);
        
        await SendEmailAsync(
            email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
    }

    public async Task SendWelcomeEmailAsync(
        string email,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.WelcomeTourist(userName);
        await SendEmailAsync(
            email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
    }

    public async Task SendWelcomeModeratorAsync(
        string firstName,
        string email,
        string tempPassword,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.WelcomeModerator(firstName, email, tempPassword);
        await SendEmailAsync(
            email, firstName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
    }

    public async Task SendAdminPromotionEmailAsync(
        string email,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.AdminPromotion(userName);
        
        await SendEmailAsync(
            email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
    }

    public async Task SendAdminDemotionEmailAsync(
        string email,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.AdminDemotion(userName);
        
        await SendEmailAsync(
            email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
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
            email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
    }

    public async Task SendReviewResponseNotificationAsync(
        string email,
        string userName,
        string siteName,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.ReviewResponse(userName, siteName);
        
        await SendEmailAsync(
            email, userName, template.Subject, template.HtmlBody, template.TextBody, cancellationToken);
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
            
            message.From.Add(new MailboxAddress(_options.FromName, _options.FromEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            
            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = textBody
            };
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            
            await client.ConnectAsync(
                _options.SmtpHost,
                _options.SmtpPort,
                false,
                cancellationToken);

            // Authenticate (if credentials provided)
            if (!string.IsNullOrEmpty(_options.SmtpUsername))
            {
                await client.AuthenticateAsync(
                    _options.SmtpUsername,
                    _options.SmtpPassword,
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
