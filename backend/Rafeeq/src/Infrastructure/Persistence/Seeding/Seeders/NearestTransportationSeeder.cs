using Application.Commands.Sites.NearestTransportations;
using Application.Commands.Sponsors.Offers;
using Application.Common.Interfaces.Services;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeding.Seeders;

/// <summary>
/// Seeds <see cref="NearestTransportation"/> entities onto existing <see cref="Site"/> aggregates
/// from <c>Persistence/Seeding/Data/NearestTransportations.csv</c> (embedded resource).
///
/// Depends on <see cref="SiteSeeder"/> (order 20) having already run.
///
/// Idempotency: a transportation entry is skipped when a record with the same English
/// name already exists for that site.
/// </summary>
internal sealed class NearestTransportationSeeder(
    ApplicationDbContext dbContext,
    ICsvFileParser csvParser,
    ILogger<NearestTransportationSeeder> logger) : IDataSeeder
{
    public int Order => 40;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = EmbeddedResourceHelper.GetCsvStream("NearestTransportations.csv");
        var rows = csvParser.ParseCsv<NearestTransportationCsvRowDto>(stream);

        if (rows.Count == 0)
        {
            logger.LogWarning("{Seeder}: NearestTransportations.csv is empty — nothing to seed.", nameof(NearestTransportationSeeder));
            return;
        }

        var sites = await dbContext.Sites
            .AsSplitQuery()
            .Include(s => s.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .Include(s => s.NearestTransportations)
                .ThenInclude(t => t.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .ToListAsync(cancellationToken);

        var siteByEnName = sites
            .Select(s => new
            {
                Site = s,
                EnName = s.LocalizedContents
                    .FirstOrDefault()?.Name
            })
            .Where(x => x.EnName is not null)
            .ToDictionary(x => x.EnName!, x => x.Site, StringComparer.OrdinalIgnoreCase);

        int addedCount = 0;

        foreach (var row in rows)
        {
            if (!siteByEnName.TryGetValue(row.SiteName.Trim(), out var site))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping transport '{Name}' — site '{Site}' not found.",
                    nameof(NearestTransportationSeeder), row.NameEn, row.SiteName);
                continue;
            }

            // Idempotency: skip if same English name already exists on this site.
            bool alreadyExists = site.NearestTransportations
                .Any(t => t.LocalizedContents
                    .Any(lc => lc.Language == LanguageCode.English &&
                               string.Equals(lc.Name, row.NameEn.Trim(), StringComparison.OrdinalIgnoreCase)));

            if (alreadyExists)
                continue;

            if (!Enum.TryParse<TransportationType>(row.Type, ignoreCase: true, out var transportType))
            {
                logger.LogWarning(
                    "{Seeder}: Unknown TransportationType '{Type}' for '{Name}' — skipped.",
                    nameof(NearestTransportationSeeder), row.Type, row.NameEn);
                continue;
            }

            var locationResult = GeoLocation.Create(row.Latitude, row.Longitude);
            if (locationResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Invalid geo-location for transport '{Name}': {Error}",
                    nameof(NearestTransportationSeeder), row.NameEn, locationResult.Error);
                continue;
            }

            var addResult = site.AddNearestTransportation(transportType, locationResult.Value, row.DistanceKm);
            if (addResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Could not add transport '{Name}' to site '{Site}': {Error}",
                    nameof(NearestTransportationSeeder), row.NameEn, row.SiteName, addResult.Error);
                continue;
            }

            var transportation = addResult.Value;

            // --- Operational flags ---
            transportation.SetOperationalStatus(row.IsOperational);
            transportation.SetAccessibility(row.HasAccessibility);

            // --- Operating hours ---
            if (!string.IsNullOrWhiteSpace(row.OperatingHours))
            {
                var parts = row.OperatingHours.Split('-');
                if (parts.Length == 2 &&
                    TimeOnly.TryParseExact(parts[0].Trim(), "HH:mm", out var opStart) &&
                    TimeOnly.TryParseExact(parts[1].Trim(), "HH:mm", out var opEnd))
                {
                    var timeRangeResult = TimeRange.Create(opStart, opEnd, differentDays: opEnd < opStart);
                    if (!timeRangeResult.Failed)
                        transportation.SetOperatingHours(timeRangeResult.Value);
                }
                else
                {
                    logger.LogWarning(
                        "{Seeder}: Cannot parse OperatingHours '{Hours}' for transport '{Name}' — skipped.",
                        nameof(NearestTransportationSeeder), row.OperatingHours, row.NameEn);
                }
            }

            // --- Localized content: English ---
            Address? addressEn = null;
            if (!string.IsNullOrWhiteSpace(row.AddressEn))
            {
                var addrResult = Address.Create(row.AddressEn);
                if (!addrResult.Failed) addressEn = addrResult.Value;
            }

            transportation.AddLocalizedContent(LanguageCode.English, row.NameEn, row.DescriptionEn, addressEn);

            // --- Localized content: Arabic ---
            if (!string.IsNullOrWhiteSpace(row.NameAr))
            {
                Address? addressAr = null;
                if (!string.IsNullOrWhiteSpace(row.AddressAr))
                {
                    var addrResult = Address.Create(row.AddressAr);
                    if (!addrResult.Failed) addressAr = addrResult.Value;
                }

                transportation.AddLocalizedContent(LanguageCode.Arabic, row.NameAr, row.DescriptionAr, addressAr);
            }

            await dbContext.AddAsync(transportation, cancellationToken);

            addedCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Seeder}: Committed {Count} transportation(s).", nameof(NearestTransportationSeeder), addedCount);
    }
}
