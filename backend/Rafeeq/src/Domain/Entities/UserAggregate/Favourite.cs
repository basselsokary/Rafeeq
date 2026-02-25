using Domain.Common;
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities.TouristAggregate;

public class Favourite : BaseAuditableEntity
{
    public Guid EntityId { get; private set; }
    public FavouriteType Type { get; set; }

    public DateTime AddedAt { get; private set; }
    public string? Notes { get; private set; }

    private Favourite() { }
    private Favourite(Guid entityId)
    {
        EntityId = entityId;

        AddedAt = DateTime.UtcNow;
    }

    public static Favourite Create(Guid entityId)
    {
        if (entityId == Guid.Empty)
            throw new BusinessRuleValidationException("Entity ID cannot be empty.");

        return new Favourite(entityId);
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
        MarkAsUpdated();
    }
}

