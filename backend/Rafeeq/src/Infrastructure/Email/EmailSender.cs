using Application.Common.Interfaces.Email;

namespace Infrastructure.Email;

public class EmailSender : IEmailSender
{
    public Task SendEmailAsync(string email, string subject, string body)
    {
        // TODO
        return Task.CompletedTask;
    }
}