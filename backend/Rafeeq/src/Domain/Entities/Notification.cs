using Domain.Common;

namespace EventMaster.Domain.Entities;

public class Notification : BaseEntity
{
    private Notification() { }
    private Notification(string userId, string title, string? message, string? sendBy)
    {
        UserId = userId;

        Title = title;
        Message = message;

        IsRead = false;

        SendAt = DateTime.UtcNow;
        SendBy = sendBy;
    }

    public string UserId { get; private set; } = null!;

    public string Title { get; private set; } = null!;
    public string? Message { get; private set; }

    public bool IsRead { get; private set; }

    public DateTime SendAt { get; private set; }
    public string? SendBy { get; private set; }

    public static Notification Create(
        string userId,
        string title,
        string? message = default,
        string? sendBy = default)
    {
        if (string.IsNullOrWhiteSpace(userId))
            throw new ArgumentException("User ID cannot be null or empty.", nameof(userId));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be null or empty.", nameof(title));

        return new(userId, title, message, sendBy);
    }
}
