using Domain.Common;
using Domain.Common.Exceptions;

namespace Domain.Entities.TouristAggregate;

public class FavouriteAttraction : BaseAuditableEntity
{
    public Guid AttractionId { get; private set; }

    public DateTime AddedAt { get; private set; }
    public string? Notes { get; private set; }

    private FavouriteAttraction() { }
    private FavouriteAttraction(Guid placeId)
    {
        AttractionId = placeId;

        AddedAt = DateTime.UtcNow;
    }

    public static FavouriteAttraction Create(Guid placeId)
    {
        if (placeId == Guid.Empty)
            throw new BusinessRuleValidationException("Place ID cannot be empty.");

        return new FavouriteAttraction(placeId);
    }

    public void AddNotes(string notes)
    {
        Notes = notes?.Trim();
        MarkAsUpdated();
    }
}