using Domain.Common;
using Domain.Exceptions;
using Domain.Enums;
using Domain.Common.Interfaces;

namespace Domain.Entities.TouristAggregate;

public class User : BaseAuditableEntity, IAggregateRoot
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;

    public UserRole Role { get; private set; }
    public UserStatus Status { get; private set; }
    public LanguageCode PreferredLanguage { get; private set; }
    
    public int TotalTrips { get; private set; }
    public int TotalReviews { get; private set; }

    private readonly List<Favourite> _favourites = [];
    public IReadOnlyCollection<Favourite> Favourites => _favourites.AsReadOnly();

    private User() { }
    private User(
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

    public static User Create(
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

        var user = new User(
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

    public void AddFavorite(Guid entityId)
    {
        if (_favourites.Any(f => f.EntityId == entityId))
            throw new BusinessRuleValidationException("Entity is already in favorites.");

        var favorite = Favourite.Create(entityId);
        _favourites.Add(favorite);
        MarkAsUpdated();
    }

    public void RemoveFavorite(Guid entityId)
    {
        var favorite = _favourites.FirstOrDefault(f => f.EntityId == entityId)
            ?? throw new EntityNotFoundException(nameof(Favourite), entityId);

        _favourites.Remove(favorite);
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

    public bool IsFavorite(Guid entityId)
    {
        return _favourites.Any(f => f.EntityId == entityId);
    }
}
