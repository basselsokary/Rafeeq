namespace Infrastructure.Emails;

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