using System.Net;
using System.Net.Mail;
using Application.Common.Interfaces.Email;
using Application.DTOs.Email;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Services;

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
        
        await SendEmailAsync(email, template.Subject, template.HtmlBody, template.TextBody);
    }

    public async Task SendEmailVerificationAsync(
        string email,
        string verificationToken,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var verificationLink = $"{_settings.ApplicationUrl}/verify-email?token={verificationToken}";
        var template = EmailTemplates.EmailVerification(userName, verificationLink);
        
        await SendEmailAsync(email, template.Subject, template.HtmlBody, template.TextBody);
    }

    public async Task SendWelcomeEmailAsync(
        string email,
        string userName,
        CancellationToken cancellationToken = default)
    {
        var template = EmailTemplates.Welcome(userName);
        await SendEmailAsync(email, template.Subject, template.HtmlBody, template.TextBody);
    }

    private async Task SendEmailAsync(
        string toEmail,
        string subject,
        string htmlBody,
        string textBody)
    {
        try
        {
            using var client = new SmtpClient(_settings.SmtpHost, _settings.SmtpPort)
            {
                Credentials = new NetworkCredential(_settings.SmtpUsername, _settings.SmtpPassword),
                EnableSsl = _settings.EnableSsl
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_settings.FromEmail, _settings.FromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };

            mailMessage.To.Add(toEmail);

            // Add plain text alternative
            var plainTextView = AlternateView.CreateAlternateViewFromString(
                textBody, 
                null, 
                "text/plain");
            mailMessage.AlternateViews.Add(plainTextView);

            await client.SendMailAsync(mailMessage);
            logger.LogInformation("Email sent successfully to {Email}", toEmail);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send email to {Email}", toEmail);
            throw;
        }
    }
}

public class EmailSettings
{
    public string SmtpHost { get; set; } = null!;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = null!;
    public string SmtpPassword { get; set; } = null!;
    public bool EnableSsl { get; set; }
    public string FromEmail { get; set; } = null!;
    public string FromName { get; set; } = null!;
    public string ApplicationUrl { get; set; } = null!;
}