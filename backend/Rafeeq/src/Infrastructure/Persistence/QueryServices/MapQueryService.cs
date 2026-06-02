using Application.Common.Interfaces.QueryServices;
using Infrastructure.Persistence.ApplicationContext;
using Application.DTOs.Common;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.QueryServices;

internal sealed class MapQueryService(
    IDbContextFactory<ApplicationDbContext> dbContextFactory) : IMapQueryService
{
    private readonly IDbContextFactory<ApplicationDbContext> _factory = dbContextFactory;

    public async Task<List<MapPlaceMarkerDto>> GetNearbyMarkersAsync(
        double latitude,
        double longitude,
        int radiusKm = 40,
        int count = 10,
        LanguageCode language = LanguageCode.English,
        CancellationToken cancellationToken = default)
    {
        await using var sitesDb = await _factory.CreateDbContextAsync(cancellationToken);
        await using var sponsorsDb = await _factory.CreateDbContextAsync(cancellationToken);

        var sitesQuery = () => sitesDb.Sites.AsNoTracking()
            .Where(a => a.Status == SiteStatus.Active)
            .Select(s => new
            {
                Site = s,
                Distance = 12742.0 * Math.Asin(
                    Math.Sqrt(
                        Math.Pow(Math.Sin((latitude - s.Location.Latitude) * Math.PI / 180 / 2.0), 2) +
                        Math.Cos(latitude * Math.PI / 180.0) *
                        Math.Cos(s.Location.Latitude * Math.PI / 180.0) *
                        Math.Pow(Math.Sin((longitude - s.Location.Longitude) * Math.PI / 180 / 2.0), 2)
                    )
                )
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .ThenByDescending(s => s.Site.IsFeatured)
            .Take(count)
            .Select(s => new MapPlaceMarkerDto(
                Id: s.Site.Id,
                Name: s.Site.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Name)
                    .FirstOrDefault()!,
                Location: new LocationDto(s.Site.Location.Latitude, s.Site.Location.Longitude),
                Type: MarkerType.Site,
                ImageUrl: s.Site.MainImageUrl
            )).ToListAsync(cancellationToken);

        var sponsorsQuery = () => sponsorsDb.Sponsors.AsNoTracking()
            .Where(s => s.Status == SponsorStatus.Active)
            .Select(s => new
            {
                Sponsor = s,
                Distance = 12742.0 * Math.Asin(
                    Math.Sqrt(
                        Math.Pow(Math.Sin((latitude - s.Location.Latitude) * Math.PI / 180 / 2.0), 2) +
                        Math.Cos(latitude * Math.PI / 180.0) *
                        Math.Cos(s.Location.Latitude * Math.PI / 180.0) *
                        Math.Pow(Math.Sin((longitude - s.Location.Longitude) * Math.PI / 180 / 2.0), 2)
                    )
                )
            })
            .Where(x => x.Distance <= radiusKm)
            .OrderBy(x => x.Distance)
            .ThenByDescending(s => s.Sponsor.Tier)
            .Take(count / 2)
            .Select(s => new MapPlaceMarkerDto(
                Id: s.Sponsor.Id,
                Name: s.Sponsor.LocalizedContents
                    .Where(lc => lc.Language == language || lc.Language == LanguageCode.English)
                    .OrderBy(lc => lc.Language == language ? 0 : 1)
                    .Select(lc => lc.Title)
                    .FirstOrDefault()!,
                Location: new LocationDto(s.Sponsor.Location.Latitude, s.Sponsor.Location.Longitude),
                Type: MarkerType.Sponsor,
                ImageUrl: s.Sponsor.MainImageUrl
            )).ToListAsync(cancellationToken);

        var sitesTask = sitesQuery();
        var sponsorsTask = sponsorsQuery();

        await Task.WhenAll(sitesTask, sponsorsTask);

        var siteMarkers = await sitesTask;
        var sponsorMarkers = await sponsorsTask;

        var allMarkers = siteMarkers.Concat(sponsorMarkers)
            .OrderBy(x => x.Type == MarkerType.Site)
            .ToList();

        return allMarkers;
    }
}
