using Domain.Common;
using Domain.Common.Exceptions;
using Domain.Enums;
using Domain.Common.Interfaces;

namespace Domain.Entities.TouristAggregate;

public class Tourist : BaseAuditableEntity, IAggregateRoot
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;

    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public LanguageCode PreferredLanguage { get; private set; }
    
    public int TotalTrips { get; private set; }
    public int TotalReviews { get; private set; }

    private readonly List<FavouriteAttraction> _favouriteAttractions = [];
    public IReadOnlyCollection<FavouriteAttraction> FavouriteAttractions => _favouriteAttractions.AsReadOnly();

    private Tourist() { }
    private Tourist(
        Guid id,
        string firstName,
        string lastName,
        UserRole role,
        LanguageCode preferredLanguage) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Role = role;
        PreferredLanguage = preferredLanguage;

        Status = UserStatus.Active;
        TotalTrips = 0;
        TotalReviews = 0;
    }

    public static Tourist Create(
        string firstName,
        string lastName,
        UserRole role = UserRole.Tourist,
        LanguageCode preferredLanguage = LanguageCode.English,
        Guid? userId = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new BusinessRuleValidationException("First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new BusinessRuleValidationException("Last name cannot be empty.");

        var user = new Tourist(
            userId ?? Guid.NewGuid(),
            firstName.Trim(),
            lastName.Trim(),
            role,
            preferredLanguage);

        return user;
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public void UpdateProfile(string firstName, string lastName, string? bio, string? nationality, DateTime? dateOfBirth)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new BusinessRuleValidationException("First name cannot be empty.");

        if (string.IsNullOrWhiteSpace(lastName))
            throw new BusinessRuleValidationException("Last name cannot be empty.");

        if (dateOfBirth.HasValue && dateOfBirth.Value > DateTime.UtcNow)
            throw new BusinessRuleValidationException("Date of birth cannot be in the future.");

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        MarkAsUpdated();
    }

    public void SetPreferredLanguage(LanguageCode language)
    {
        PreferredLanguage = language;
        MarkAsUpdated();
    }

    public void UpdateStatus(UserStatus status)
    {
        if (Status == status) return;

        Status = status;
        MarkAsUpdated();
        // RaiseDomainEvent(new UserStatusChangedEvent(Id, status));
    }

    public void AddFavoriteAttraction(Guid attractionId)
    {
        if (_favouriteAttractions.Any(f => f.AttractionId == attractionId))
            throw new BusinessRuleValidationException("Attraction is already in favorites.");

        var favorite = FavouriteAttraction.Create(attractionId);
        _favouriteAttractions.Add(favorite);
        MarkAsUpdated();
    }

    public void RemoveFavoriteAttraction(Guid attractionId)
    {
        var favorite = _favouriteAttractions.FirstOrDefault(f => f.AttractionId == attractionId)
            ?? throw new EntityNotFoundException(nameof(FavouriteAttraction), attractionId);

        _favouriteAttractions.Remove(favorite);
        MarkAsUpdated();
    }

    public void IncrementTripCount()
    {
        TotalTrips++;
        MarkAsUpdated();
    }

    public void IncrementReviewCount()
    {
        TotalReviews++;
        MarkAsUpdated();
    }

    public bool IsFavoriteAttraction(Guid attractionId)
    {
        return _favouriteAttractions.Any(f => f.AttractionId == attractionId);
    }
}
