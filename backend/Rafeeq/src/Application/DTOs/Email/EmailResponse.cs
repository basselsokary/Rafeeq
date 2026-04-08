namespace Application.DTOs.Email;

public record EmailResponse(string Subject, string HtmlBody, string TextBody);

public static class EmailTemplates
{
    public static EmailResponse PasswordReset(string userName, string resetLink)
    {
        return new EmailResponse(
            Subject: "Reset Your Rafeeq Password",
            HtmlBody: $@"
                <h2>Password Reset Request</h2>
                <p>Hello {userName},</p>
                <p>You requested to reset your password for your Rafeeq account.</p>
                <p>Click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>This link will expire in 24 hours.</p>
                <p>If you didn't request this, please ignore this email.</p>
                <p>Best regards,<br/>The Rafeeq Team</p>
            ",
            TextBody: $@"
                Password Reset Request
                
                Hello {userName},
                
                You requested to reset your password for your Rafeeq account.
                
                Click the link below to reset your password:
                {resetLink}
                
                This link will expire in 24 hours.
                
                If you didn't request this, please ignore this email.
                
                Best regards,
                The Rafeeq Team
            ");
    }

    public static EmailResponse EmailVerification(string userName, string verificationLink)
    {
        return new EmailResponse(
            Subject: "Verify Your Rafeeq Email",
            HtmlBody: $@"
                <h2>Verify Your Email</h2>
                <p>Hello {userName},</p>
                <p>Welcome to Rafeeq! Please verify your email address.</p>
                <p>Click the link below to verify:</p>
                <p><a href='{verificationLink}'>Verify Email</a></p>
                <p>This link will expire in 48 hours.</p>
                <p>Best regards,<br/>The Rafeeq Team</p>
            ",
            TextBody: $@"
                Verify Your Email
                
                Hello {userName},
                
                Welcome to Rafeeq! Please verify your email address.
                
                Click the link below to verify:
                {verificationLink}
                
                This link will expire in 48 hours.
                
                Best regards,
                The Rafeeq Team
            ");
    }

    public static EmailResponse Welcome(string userName)
    {
        return new EmailResponse(
            Subject: "Welcome to Rafeeq!",
            HtmlBody: $@"
                <h2>Welcome to Rafeeq!</h2>
                <p>Hello {userName},</p>
                <p>Thank you for joining Rafeeq, your personal guide to Egypt's amazing attractions!</p>
                <p>Start exploring historical sites, plan your trips, and discover hidden gems.</p>
                <p>Best regards,<br/>The Rafeeq Team</p>
            ",
            TextBody: $@"
                Welcome to Rafeeq!
                
                Hello {userName},
                
                Thank you for joining Rafeeq, your personal guide to Egypt's amazing attractions!
                
                Start exploring historical sites, plan your trips, and discover hidden gems.
                
                Best regards,
                The Rafeeq Team
            ");
    }
}
