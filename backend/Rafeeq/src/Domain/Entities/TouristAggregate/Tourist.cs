using Domain.Common;
using Domain.Enums;
using Domain.Common.Interfaces;
using Shared;

namespace Domain.Entities.TouristAggregate;

public class Tourist : BaseEntity, IAggregateRoot
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;
    public string? Nationality { get; private set; }

    public UserStatus Status { get; private set; }
    
    public int TotalTrips { get; private set; }
    public int TotalReviews { get; private set; }

    public DateTime CreatedAt { get; protected set; }
    public DateTime? LastModifiedAt { get; protected set; }

    private readonly List<FavouriteSite> _favourites = [];
    public IReadOnlyCollection<FavouriteSite> Favourites => _favourites.AsReadOnly();
    
    private readonly List<VisitedSite> _visitedSites = [];
    public IReadOnlyCollection<VisitedSite> VisitedSites => _visitedSites.AsReadOnly();

    private Tourist() { }
    private Tourist(
        Guid id,
        string firstName,
        string lastName,
        string? nationality = null) : base(id)
    {
        FirstName = firstName;
        LastName = lastName;
        Nationality = nationality;

        Status = UserStatus.Active;
        TotalTrips = 0;
        TotalReviews = 0;

        CreatedAt = DateTime.UtcNow;
    }

    public static Result<Tourist> Create(
        Guid TouristId,
        string firstName,
        string lastName,
        string email,
        string? nationality = null)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            return TouristErrors.FirstNameRequired;

        if (string.IsNullOrWhiteSpace(lastName))
            return TouristErrors.LastNameRequired;
   
        var tourist = new Tourist(
            TouristId,
            firstName.Trim(),
            lastName.Trim(),
            nationality?.Trim());
        
        tourist.RaiseDomainEvent(new TouristRegisteredEvent(email, firstName));

        return tourist;
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

        UpdateLastModified();
        RaiseDomainEvent(new TouristProfileUpdatedEvent(Id, firstName, lastName));

        return Result.Success();
    }

    public void UpdateStatus(UserStatus status)
    {
        if (Status == status) return;

        Status = status;
        UpdateLastModified();
        // RaiseDomainEvent(new TouristStatusChangedEvent(Id, status));
    }

    public Result<VisitedSite> MarkSiteAsVisited(Guid siteId, int durationMinutes, DateTime? visitDate)
    {
        if (_visitedSites.Any(v => v.SiteId == siteId))
            return TouristErrors.SiteAlreadyVisited;

        var visitedSiteResult = VisitedSite.Create(siteId, durationMinutes, visitDate);
        if (visitedSiteResult.Failed)
            return visitedSiteResult;

        _visitedSites.Add(visitedSiteResult.Value);
        UpdateLastModified();

        return Result.Success(visitedSiteResult.Value);
    }

    public Result RemoveVisitedSite(Guid siteId)
    {
        var visitedSite = _visitedSites.FirstOrDefault(v => v.SiteId == siteId);
        if (visitedSite == null)
            return TouristErrors.VisitedSiteNotFound;

        _visitedSites.Remove(visitedSite);
        UpdateLastModified();

        return Result.Success();
    }

    public Result<FavouriteSite> AddFavorite(Guid siteId)
    {
        if (_favourites.Any(f => f.SiteId == siteId))
            return TouristErrors.SiteAlreadyFavorite;

        var favoriteResult = FavouriteSite.Create(siteId);
        if (favoriteResult.Failed)
            return favoriteResult;

        _favourites.Add(favoriteResult.Value);
        UpdateLastModified();

        RaiseDomainEvent(new TouristFavoriteAddedEvent(Id, siteId));
        return Result.Success(favoriteResult.Value);
    }

    public Result RemoveFavorite(Guid siteId)
    {
        var favorite = _favourites.FirstOrDefault(f => f.SiteId == siteId);
        if (favorite == null)
            return TouristErrors.FavouriteNotFound;

        _favourites.Remove(favorite);
        UpdateLastModified();

        RaiseDomainEvent(new TouristFavoriteRemovedEvent(Id, siteId));
        return Result.Success();
    }

    public void IncrementTripCount()
    {
        TotalTrips++;
        UpdateLastModified();
    }

    public void IncrementReviewCount()
    {
        TotalReviews++;
        UpdateLastModified();
    }

    public void DecrementReviewCount()
    {
        if (TotalReviews > 0)
        {
            TotalReviews--;
            UpdateLastModified();
        }
    }

    public bool IsFavorite(Guid siteId)
    {
        return _favourites.Any(f => f.SiteId == siteId);
    }

    private void UpdateLastModified(DateTime? modifiedAt = null)
    {
        if (modifiedAt.HasValue)
        {
            LastModifiedAt = modifiedAt.Value;
        }

        LastModifiedAt = DateTime.UtcNow;
    }
}
