using Application.Common.Interfaces.Authentication;
using Application.Common.Interfaces.Email;
using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;
using Domain.Entities.TouristAggregate;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers;

internal class RatingAddedEventHandler(
    IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<RatingAddedEventHandler> logger) : IDomainEventHandler<RatingAddedEvent>
{
    public async Task HandleAsync(
        RatingAddedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for RatingAddedEvent with VisitedSiteId {VisitedSiteId} and User ID {UserId}",
                domainEvent.SiteId, domainEvent.VisitedSiteId, domainEvent.UserId);

            return;
        }

        site.AddRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveByIdAsync("tourist:list:favorites", domainEvent.SiteId.ToString());
    }
}

internal class RatingUpdatedEventHandler(
    IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<RatingUpdatedEventHandler> logger) : IDomainEventHandler<RatingUpdatedEvent>
{
    public async Task HandleAsync(RatingUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for RatingUpdatedEvent with VisitedSite ID {VisitedSiteId}",
                domainEvent.SiteId, domainEvent.VisitedSiteId);

            return;
        }

        site.UpdateRating(domainEvent.OldRating, domainEvent.NewRating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveByIdAsync("tourist:list:favorites", domainEvent.SiteId.ToString());
    }
}

internal class RatingRemovedEventHandler(
    IUnitOfWork unitOfWork, ICacheService cacheService, ILogger<RatingRemovedEventHandler> logger) : IDomainEventHandler<RatingRemovedEvent>
{
    public async Task HandleAsync(RatingRemovedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        var site = await unitOfWork.Sites.GetByIdAsync(domainEvent.SiteId, cancellationToken);
        if (site == null)
        {
            logger.LogWarning(
                "Site with ID {SiteId} not found for RatingRemovedEvent with VisitedSite ID {VisitedSiteId}",
                domainEvent.SiteId, domainEvent.VisitedSiteId);

            return;
        }

        site.RemoveRating(domainEvent.Rating);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        await cacheService.RemoveByIdAsync("tourist:list:favorites", domainEvent.SiteId.ToString());
    }
}

internal class UserRegisteredEventHandler(
    IEmailService emailService, IIdentityService identityService, ILogger<UserRegisteredEventHandler> logger) : IDomainEventHandler<TouristRegisteredEvent>
{
    public async Task HandleAsync(TouristRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Result<(string ResetToken, string UserName)> result = await identityService.GenerateEmailConfirmationTokenAsync(domainEvent.Email);
        if (result.Failed)
        {
            logger.LogError(
                "Failed to generate email confirmation token for user {Email}: {Error}",
                domainEvent.Email,
                result.Error.Message);

            return;
        }

        await emailService.SendEmailVerificationAsync(
            domainEvent.Email,
            result.Value.ResetToken,
            domainEvent.FirstName,
            cancellationToken);
    }
}

internal class TouristProfileUpdatedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<TouristProfileUpdatedEvent>
{
    public async Task HandleAsync(TouristProfileUpdatedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        Console.WriteLine($"Handling TouristProfileUpdatedEvent for Tourist ID: {domainEvent.TouristId}");
        await cacheService.RemoveByIdAsync("tourist", domainEvent.TouristId.ToString());
    }
}

internal class TouristFavoriteAddedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<TouristFavoriteAddedEvent>
{
    public async Task HandleAsync(TouristFavoriteAddedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await cacheService.RemoveByIdAsync("tourist:list:favorites", domainEvent.TouristId.ToString());
    }
}

internal class TouristFavoriteRemovedEventHandler(
    ICacheService cacheService) : IDomainEventHandler<TouristFavoriteRemovedEvent>
{
    public async Task HandleAsync(TouristFavoriteRemovedEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await cacheService.RemoveByIdAsync("tourist:list:favorites", domainEvent.TouristId.ToString());
    }
}