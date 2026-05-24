using Application.Commands.Sites;
using Application.Commands.Sponsors;
using Application.Common.Interfaces.Services;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Persistence.ApplicationContext;
using Infrastructure.Persistence.Seeding.Parsers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeding.Seeders;

/// <summary>
/// Seeds <see cref="Site"/> records (including their facilities) from
/// <c>Persistence/Seeding/Data/Sites.csv</c> (embedded resource).
///
/// Depends on <see cref="CitySeeder"/> (order 10) having already run so that
/// cities are available for FK resolution.
///
/// Idempotency: a site is skipped when an English localized-content row with the
/// same name already exists in the database.
///
/// Note: Opening hours, attractions, artifacts, and nearest transportations are
/// seeded by their own dedicated seeders (orders 30–60).
/// </summary>
internal sealed class SiteSeeder(
    ApplicationDbContext dbContext,
    ICsvFileParser csvParser,
    ILogger<SiteSeeder> logger) : IDataSeeder
{
    public int Order => 20;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = EmbeddedResourceHelper.GetCsvStream("Sites.csv");
        var rows = csvParser.ParseCsv<SiteCsvRowDto>(stream);

        if (rows.Count == 0)
        {
            logger.LogWarning("{Seeder}: Sites.csv is empty — nothing to seed.", nameof(SiteSeeder));
            return;
        }

        // Build a city name → city entity lookup.
        var cities = await dbContext.Cities
            .Include(c => c.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .ToListAsync(cancellationToken);

        var cityByName = cities
            .Select(c => new
            {
                City = c,
                EnName = c.LocalizedContents
                    .FirstOrDefault()?.Name
            })
            .Where(x => x.EnName is not null)
            .ToDictionary(x => x.EnName!, x => x.City, StringComparer.OrdinalIgnoreCase);

        // Idempotency: skip already-seeded sites by English name.
        var existingNames = await dbContext.Sites
            .SelectMany(s => s.LocalizedContents)
            .Where(lc => lc.Language == LanguageCode.English)
            .Select(lc => lc.Name)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, cancellationToken);

        var toSeed = rows
            .Where(r => !existingNames.Contains(r.NameEn.Trim()))
            .ToList();

        if (toSeed.Count == 0)
        {
            logger.LogInformation("{Seeder}: All sites already seeded.", nameof(SiteSeeder));
            return;
        }

        logger.LogInformation("{Seeder}: Seeding {Count} new site(s).", nameof(SiteSeeder), toSeed.Count);

        foreach (var row in toSeed)
        {
            // --- City FK resolution ---
            if (!cityByName.TryGetValue(row.CityName.Trim(), out var city))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping site '{Name}' — city '{City}' not found.",
                    nameof(SiteSeeder), row.NameEn, row.CityName);
                continue;
            }

            // --- Value objects ---
            var locationResult = GeoLocation.Create(row.Latitude, row.Longitude);
            if (locationResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping site '{Name}' — invalid geo-location: {Error}",
                    nameof(SiteSeeder), row.NameEn, locationResult.Error);
                continue;
            }

            var addressEnResult = Address.Create(row.AddressEn);
            if (addressEnResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping site '{Name}' — invalid English address: {Error}",
                    nameof(SiteSeeder), row.NameEn, addressEnResult.Error);
                continue;
            }

            if (!Enum.TryParse<SiteType>(row.Type, ignoreCase: true, out var siteType))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping site '{Name}' — unknown SiteType '{Type}'.",
                    nameof(SiteSeeder), row.NameEn, row.Type);
                continue;
            }

            if (!Enum.TryParse<SiteStatus>(row.Status, ignoreCase: true, out var siteStatus))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping site '{Name}' — unknown SiteStatus '{Status}'.",
                    nameof(SiteSeeder), row.NameEn, row.Status);
                continue;
            }

            // --- Create site ---
            var siteResult = Site.Create(
                cityId: city.Id,
                name: row.NameEn,
                description: row.DescriptionEn,
                address: addressEnResult.Value,
                entryFeeNotes: row.EntryFeeNoteEn,
                location: locationResult.Value,
                type: siteType,
                estimatedDurationMinutes: row.EstimatedDurationMinutes,
                contactPhone: row.ContactPhone,
                contactWebsiteUrl: row.WebsiteUrl);

            if (siteResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping site '{Name}' — domain validation failed: {Error}",
                    nameof(SiteSeeder), row.NameEn, siteResult.Error);
                continue;
            }

            var site = siteResult.Value;

            // ── Status & visibility flags ───────────────────────────────────
            site.UpdateStatus(siteStatus, row.IsHiddenGem, row.IsFeatured);

            // ── Entry fee ───────────────────────────────────────────────────
            // Column is nullable. Empty → no ticket. "foreignAdult,localAdult" → Ticket.
            // Local price is always EGP; foreign price may carry a currency symbol.
            var ticketParseResult = EntryFeeParser.TryParse(row.EntryFee);

            if (ticketParseResult is null)
            {
                // No cell value — honour the Is Free? flag.
                if (row.IsFree)
                    site.RemoveEntryFee(isFree: true);
                // else: site remains with no ticket and IsFree = false (unknown / TBD).
            }
            else if (ticketParseResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Could not parse entry fee '{Raw}' for site '{Name}': {Error} — site saved without ticket.",
                    nameof(SiteSeeder), row.EntryFee, row.NameEn, ticketParseResult.Error);
            }
            else
            {
                var ticket = ticketParseResult.Value;

                // If both amounts are zero the site is effectively free.
                if (ticket.EgyptianPrice.Amount == 0 &&
                    (ticket.ForeignerPrice is null || ticket.ForeignerPrice.Amount == 0))
                {
                    site.RemoveEntryFee(isFree: true);
                }
                else
                {
                    site.SetEntryFee(ticket);
                }
            }

            // --- Arabic localized content ---
            if (!string.IsNullOrWhiteSpace(row.NameAr) && !string.IsNullOrWhiteSpace(row.DescriptionAr))
            {
                var addressArResult = Address.Create(
                    string.IsNullOrWhiteSpace(row.AddressAr) ? row.AddressEn : row.AddressAr);

                if (!addressArResult.Failed)
                {
                    site.AddLocalizedContent(
                        LanguageCode.Arabic,
                        row.NameAr,
                        row.DescriptionAr,
                        addressArResult.Value,
                        row.EntryFeeNoteAr);
                }
            }

            // --- Facilities ---
            if (!string.IsNullOrWhiteSpace(row.Facilities))
            {
                var facilityNames = row.Facilities
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

                foreach (var facilityName in facilityNames)
                {
                    if (Enum.TryParse<FacilityType>(facilityName, ignoreCase: true, out var facility))
                        site.AddFacility(facility);
                    else
                        logger.LogWarning(
                            "{Seeder}: Unknown FacilityType '{Facility}' on site '{Name}' — skipped.",
                            nameof(SiteSeeder), facilityName, row.NameEn);
                }
            }

            // --- Increment city counter ---
            city.IncrementSiteCount();

            await dbContext.Sites.AddAsync(site, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Seeder}: Committed {Count} site(s).", nameof(SiteSeeder), toSeed.Count);
    }
}
