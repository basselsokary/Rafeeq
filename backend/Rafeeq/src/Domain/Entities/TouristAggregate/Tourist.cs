using Domain.Common;
using Domain.Enums;
using Domain.Common.Interfaces;
using Shared.Models;

namespace Domain.Entities.TouristAggregate;

public class Tourist : BaseAuditableEntity, IAggregateRoot
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string Nationality { get; private set; } = null!;

    public TouristStatus Status { get; private set; }
    public LanguageCode PreferredLanguage { get; private set; }
    
    public int TotalTrips { get; private set; }
    public int TotalReviews { get; private set; }

    private readonly List<Favourite> _favourites = [];
    public IReadOnlyCollection<Favourite> Favourites => _favourites.AsReadOnly();

    private Tourist() { }
    private Tourist(
        Guid id,
        string firstName,
        string lastName,
        string nationality,
        LanguageCode preferredLanguage) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Nationality = nationality;
        PreferredLanguage = preferredLanguage;

        Status = TouristStatus.Active;
        TotalTrips = 0;
        TotalReviews = 0;
    }

    public static Result<Tourist> Create(
        Guid TouristId,
        string firstName,
        string lastName,
        string nationality,
        LanguageCode preferredLanguage = LanguageCode.English)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return TouristErrors.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(lastName))
            return TouristErrors.LastNameRequired;

        if (string.IsNullOrWhiteSpace(nationality))
            return TouristErrors.NationalityRequired;
        
        var Tourist = new Tourist(
            TouristId,
            firstName.Trim(),
            lastName.Trim(),
            nationality,
            preferredLanguage);

        return Tourist;
    }

    public string GetFullName() => $"{FirstName} {LastName}";

    public Result UpdateProfile(string firstName, string lastName, string nationality)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return TouristErrors.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(lastName))
            return TouristErrors.LastNameRequired;
        
        if (string.IsNullOrWhiteSpace(nationality))
            return TouristErrors.NationalityRequired;

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Nationality = nationality.Trim();
        MarkAsUpdated();

        return Result.Success();
    }

    public void SetPreferredLanguage(LanguageCode language)
    {
        if (PreferredLanguage == language) return;
        
        PreferredLanguage = language;
        MarkAsUpdated();
    }

    public void UpdateStatus(TouristStatus status)
    {
        if (Status == status) return;

        Status = status;
        MarkAsUpdated();
        // RaiseDomainEvent(new TouristStatusChangedEvent(Id, status));
    }

    public Result AddFavorite(Guid siteId)
    {
        if (_favourites.Any(f => f.SiteId == siteId))
            return TouristErrors.SiteAlreadyFavorite;

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
            return TouristErrors.FavouriteNotFound(siteId);

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
