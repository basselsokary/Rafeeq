using Domain.Common;
using EventMaster.Domain.Entities;
using EventMaster.Domain.Errors;
using Shared.Models;

namespace Domain.Entities;

public class Basket : BaseAuditableEntity, IAggregateRoot
{
    private Basket() { }
    private Basket(string participantId)
    {
        ParticipantId = participantId;
    }

    public string ParticipantId { get; private set; } = string.Empty;

    private readonly List<SavedEventItem> _savedEventItems = [];
    public IReadOnlyCollection<SavedEventItem> SavedEventItems => _savedEventItems;

    public static Basket Create(string participantId)
    {
        if (string.IsNullOrWhiteSpace(participantId))
            throw new ArgumentException("Participant ID cannot be null or empty.", nameof(participantId));

        return new(participantId);
    }

    public Result AddItem(SavedEventItem item)
    {
        if (ItemExist(item))
            return Result.Failure(BasketErrors.SavedEventAlreadyExists(item.EventId));

        _savedEventItems.Add(item);
        return Result.Success();
    }

    public Result RemoveItem(SavedEventItem item)
    {
        if (!ItemExist(item))
            return Result.Failure(BasketErrors.NotFound());

        _savedEventItems.Remove(item);
        return Result.Success();
    }

    public Result RemoveItem(Guid eventId)
    {
        var item = _savedEventItems.FirstOrDefault(se => se.EventId == eventId);
        if (item == null)
            return Result.Failure(BasketErrors.NotFound());

        _savedEventItems.Remove(item);
        return Result.Success();
    }

    private bool ItemExist(SavedEventItem item)
        => _savedEventItems.Any(se => se.EventId == item.EventId);
}
