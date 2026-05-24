namespace Application.DTOs.Email;

public record EmailResponse(string Subject, string HtmlBody, string TextBody);

public static class EmailTemplates
{
    public static EmailResponse PasswordReset(string userName, string resetLink, int expiresInHours)
    {
        return new EmailResponse(
            Subject: "Reset Your Rafeeq Password",
            HtmlBody: $@"
                <h2>Password Reset Request</h2>
                <p>Hello {userName},</p>
                <p>You requested to reset your password for your Rafeeq account.</p>
                <p>Click the link below to reset your password:</p>
                <p><a href='{resetLink}'>Reset Password</a></p>
                <p>This link will expire in {expiresInHours} hours.
                <p>If you didn't request this, please ignore this email.</p>
                <p>Best regards,<br/>The Rafeeq Team</p>
            ",
            TextBody: $@"
                Password Reset Request
                
                Hello {userName},
                
                You requested to reset your password for your Rafeeq account.
                
                Click the link below to reset your password:
                {resetLink}
                
                This link will expire in {expiresInHours} hours.
                
                If you didn't request this, please ignore this email.
                
                Best regards,
                The Rafeeq Team
            ");
    }

    public static EmailResponse EmailVerification(string userName, string verificationLink, int expiresInHours)
    {
        return new EmailResponse(
            Subject: "Verify Your Rafeeq Email",
            HtmlBody: $@"
                <h2>Verify Your Email</h2>
                <p>Hello {userName},</p>
                <p>Welcome to Rafeeq! Please verify your email address.</p>
                <p>Click the link below to verify:</p>
                <p><a href='{verificationLink}'>Verify Email</a></p>
                <p>This link will expire in {expiresInHours} hours.</p>
                <p>Best regards,<br/>The Rafeeq Team</p>
            ",
            TextBody: $@"
                Verify Your Email
                
                Hello {userName},
                
                Welcome to Rafeeq! Please verify your email address.
                
                Click the link below to verify:
                {verificationLink}
                
                This link will expire in {expiresInHours} hours.
                
                Best regards,
                The Rafeeq Team
            ");
    }

    public static EmailResponse WelcomeTourist(string userName)
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
    
    public static EmailResponse WelcomeModerator(string firstName, string email, string tempPassword)
    {
        return new EmailResponse(
            Subject: "Welcome to Rafeeq Moderator Team!",
            HtmlBody: $@"
                <h2>Welcome to Rafeeq Moderator Team!</h2>
                <p>Hello {firstName},</p>
                <p>Congratulations! You have been added as a moderator to the Rafeeq platform.</p>
                <p>Your login credentials:</p>
                <ul>
                    <li><strong>Email:</strong> {email}</li>
                    <li><strong>Temporary Password:</strong> {tempPassword}</li>
                </ul>
                <p><strong>IMPORTANT:</strong> You must change your password on first login.</p>
                <p>Login here: <a href='https://admin.rafeeq.com/login'>https://admin.rafeeq.com/login</a></p>
                <p>If you did not expect this email, please contact: <a href='mailto:admin@rafeeq.live'>admin@rafeeq.live</a></p>
            ",
            TextBody: $@"
                Your login credentials:
                Email: {email}
                Temporary Password: {tempPassword}

                IMPORTANT: You must change your password on first login.

                Login here: https://admin.rafeeq.live/login

                If you did not expect this email, please contact: admin@rafeeq.live

                Best regards,
                Rafeeq Admin Team
            ");
    }

    public static EmailResponse TripReminder(string userName, string tripName, DateTime tripDate)
    {
        return new EmailResponse(
            Subject: $"Reminder: Upcoming Trip - {tripName}",
            HtmlBody: $@"
                <h2>Trip Reminder</h2>
                <p>Hello {userName},</p>
                <p>This is a friendly reminder about your upcoming trip:</p>
                <ul>
                    <li><strong>Trip Name:</strong> {tripName}</li>
                    <li><strong>Date:</strong> {tripDate:MMMM dd, yyyy}</li>
                </ul>
                <p>We hope you have a fantastic time exploring Egypt's wonders!</p>
                <p>Best regards,<br/>The Rafeeq Team</p>
            ",
            TextBody: $@"
                Trip Reminder
                
                Hello {userName},
                
                This is a friendly reminder about your upcoming trip:
                
                Trip Name: {tripName}
                Date: {tripDate:MMMM dd, yyyy}
                
                We hope you have a fantastic time exploring Egypt's wonders!
                
                Best regards,
                The Rafeeq Team
            ");
    }

    public static EmailResponse ReviewResponse(string userName, string attractionName)
    {
        return new EmailResponse(
            Subject: "Review Response",
            HtmlBody: $@"
                <h2>Review Response</h2>
                <p>Hello {userName},</p>
                <p>Your review for {attractionName} has been received and is under review.</p>
                <p>Thank you for your feedback!</p>
                <p>Best regards,<br/>The Rafeeq Team</p>
            ",
            TextBody: $@"
                Review Response
                
                Hello {userName},
                
                Your review for {attractionName} has been received and is under review.
                
                Thank you for your feedback!
                
                Best regards,
                The Rafeeq Team
            ");
    }
}
