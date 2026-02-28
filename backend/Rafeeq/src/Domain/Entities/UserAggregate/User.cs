using Domain.Common;
using Domain.Enums;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Shared.Models;

namespace Domain.Entities.UserAggregate;

public class User : BaseAuditableEntity, IAggregateRoot
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Nationality { get; private set; } = null!;

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
        string nationality,
        UserRole role,
        LanguageCode preferredLanguage) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Nationality = nationality;
        Role = role;
        PreferredLanguage = preferredLanguage;

        Status = UserStatus.Active;
        TotalTrips = 0;
        TotalReviews = 0;
    }

    public static Result<User> Create(
        Guid userId,
        string firstName,
        string lastName,
        string nationality,
        UserRole role = UserRole.Tourist,
        LanguageCode preferredLanguage = LanguageCode.English)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return UserErrors.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(lastName))
            return UserErrors.LastNameRequired;

        if (string.IsNullOrWhiteSpace(nationality))
            return UserErrors.NationalityRequired;
        
        var user = new User(
            userId,
            firstName.Trim(),
            lastName.Trim(),
            nationality,
            role,
            preferredLanguage);

        return user;
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public Result UpdateProfile(string firstName, string lastName, string nationality)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return UserErrors.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(lastName))
            return UserErrors.LastNameRequired;
        
        if (string.IsNullOrWhiteSpace(nationality))
            return UserErrors.NationalityRequired;

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Nationality = nationality.Trim();
        MarkAsUpdated();

        return Result.Success();
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

    public Result AddFavorite(Guid siteId)
    {
        if (_favourites.Any(f => f.SiteId == siteId))
            return UserErrors.SiteAlreadyFavorite;

        var favoriteResult = Favourite.Create(siteId);
        if (favoriteResult.Failed)
            return favoriteResult;

        _favourites.Add(favoriteResult.Value);
        MarkAsUpdated();

        return Result.Success();
    }

    public Result RemoveFavorite(Guid siteId)
    {
        var favorite = _favourites.FirstOrDefault(f => f.SiteId == siteId);
        if (favorite == null)
            return UserErrors.FavouriteNotFound(siteId);

        _favourites.Remove(favorite);
        MarkAsUpdated();

        return Result.Success();
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
        return _favourites.Any(f => f.SiteId == entityId);
    }
}
