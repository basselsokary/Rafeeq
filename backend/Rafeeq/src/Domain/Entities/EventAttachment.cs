using Domain.Common;

namespace EventMaster.Domain.Entities;

public class EventAttachment : BaseEntity
{
    private EventAttachment() { }
    private EventAttachment(string? text, string fileUrl, Guid eventId) : this(text, fileUrl)
    {
        EventId = eventId;
    }
    private EventAttachment(string? text, string fileUrl)
    {
        Text = text;
        FileUrl = fileUrl;

        UploadedAt = DateTime.UtcNow;
    }

    public string? Text { get; private set; }

    public string FileUrl { get; private set; } = null!;

    public DateTime UploadedAt { get; private set; }

    public Guid EventId { get; private set; }

    public static EventAttachment Create(string fileUrl, string? text, Guid eventId = default)
    {
        if (string.IsNullOrWhiteSpace(fileUrl))
            throw new ArgumentException("File URL cannot be null or empty.", nameof(fileUrl));

        if (eventId == default)
            return new(text, fileUrl);

        return new(text, fileUrl, eventId);
    }

    public override string ToString()
    {
        return FileUrl;
    }
}
