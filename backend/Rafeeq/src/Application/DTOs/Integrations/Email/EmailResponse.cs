namespace Application.DTOs.Integrations.Email;

public record EmailResponse(string Subject, string HtmlBody, string TextBody);
