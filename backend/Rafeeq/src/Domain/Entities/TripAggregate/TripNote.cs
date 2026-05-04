using Domain.Common;
using Shared;
using Domain.Entities.TouristAggregate;

namespace Domain.Entities.TripAggregate;

public class TripNote : BaseAuditableEntity
{
    public string Content { get; private set; } = null!;
    public string? Title { get; private set; }

    private TripNote() { }
    private TripNote(Guid id, string content, string? title)
    {
        Id = id;
        Content = content;
        Title = title;
    }

    public static Result<TripNote> Create(string content, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            return TouristErrors.ContentRequired;

        return new TripNote(Guid.NewGuid(), content.Trim(), title?.Trim());
    }

    public Result Update(string content, string? title = null)
    {
        if (string.IsNullOrWhiteSpace(content))
            return TouristErrors.ContentRequired;

        Content = content.Trim();
        Title = title?.Trim();
        
        return Result.Success();
    }
}
