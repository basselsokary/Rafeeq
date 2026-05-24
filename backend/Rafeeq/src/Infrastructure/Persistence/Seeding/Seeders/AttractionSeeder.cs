using Application.Commands.Attractions;
using Application.Common.Interfaces.Services;
using Domain.Entities.AttractionAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeding.Seeders;

/// <summary>
/// Seeds <see cref="Attraction"/> aggregates from
/// <c>Persistence/Seeding/Data/Attractions.csv</c> (embedded resource).
///
/// Depends on <see cref="SiteSeeder"/> (order 20) having already run.
///
/// Idempotency: an attraction is skipped when an English localized-content row
/// with the same name already exists for that site.
/// </summary>
internal sealed class AttractionSeeder(
    ApplicationDbContext dbContext,
    ICsvFileParser csvParser,
    ILogger<AttractionSeeder> logger) : IDataSeeder
{
    public int Order => 50;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = EmbeddedResourceHelper.GetCsvStream("Attractions.csv");
        var rows = csvParser.ParseCsv<AttractionCsvRowDto>(stream);

        if (rows.Count == 0)
        {
            logger.LogWarning("{Seeder}: Attractions.csv is empty — nothing to seed.", nameof(AttractionSeeder));
            return;
        }

        // Site name → Site lookup (English name as key).
        var sites = await dbContext.Sites
            .AsSplitQuery()
            .Include(s => s.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
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

        // Idempotency: load all existing English attraction names per site.
        var existingAttractionNames = await dbContext.Attractions
            .Include(a => a.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .SelectMany(a => a.LocalizedContents
                .Select(lc => new { a.SiteId, lc.Name }))
            .ToListAsync(cancellationToken);

        var existingSet = existingAttractionNames
            .Select(x => (x.SiteId, x.Name.ToUpperInvariant()))
            .ToHashSet();

        int addedCount = 0;

        foreach (var row in rows)
        {
            // ── Site FK resolution ──────────────────────────────────────────
            if (!siteByEnName.TryGetValue(row.SiteName.Trim(), out var site))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping attraction '{Name}' — site '{Site}' not found.",
                    nameof(AttractionSeeder), row.NameEn, row.SiteName);
                continue;
            }

            // ── Idempotency ─────────────────────────────────────────────────
            if (existingSet.Contains((site.Id, row.NameEn.Trim().ToUpperInvariant())))
                continue;

            // ── AttractionType ──────────────────────────────────────────────
            if (!Enum.TryParse<AttractionType>(row.Type, ignoreCase: true, out var attractionType))
            {
                logger.LogWarning(
                    "{Seeder}: Unknown AttractionType '{Type}' for attraction '{Name}' — skipped.",
                    nameof(AttractionSeeder), row.Type, row.NameEn);
                continue;
            }

            // ── Historical periods ──────────────────────────────────────────
            // Column: comma-separated HistoricalPeriod enum names.
            var historicalPeriods = new List<HistoricalPeriod>();
            if (!string.IsNullOrWhiteSpace(row.HistoricalPeriods))
            {
                foreach (var periodName in row.HistoricalPeriods
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    if (Enum.TryParse<HistoricalPeriod>(
                        periodName.Replace(" ", ""), // Allow spaces in CSV (e.g. "Early Islamic" → EarlyIslamic)
                        ignoreCase: true,
                        out var period))
                        
                        historicalPeriods.Add(period);
                    else
                        logger.LogWarning(
                            "{Seeder}: Unknown HistoricalPeriod '{Period}' on attraction '{Name}' — skipped.",
                            nameof(AttractionSeeder), periodName, row.NameEn);
                }
            }

            // ── Create attraction ───────────────────────────────────────────
            var attractionResult = Attraction.Create(
                siteId: site.Id,
                name: row.NameEn,
                description: row.DescriptionEn,
                locationDescription: row.LocationGuidEn,
                type: attractionType,
                historicalPeriods: historicalPeriods);

            if (attractionResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping attraction '{Name}' — domain validation failed: {Error}",
                    nameof(AttractionSeeder), row.NameEn, attractionResult.Error);
                continue;
            }

            var attraction = attractionResult.Value;

            // ── Featured flag ───────────────────────────────────────────────
            if (row.IsFeatured)
                attraction.SetAsFeatured(true);

            // ── Exact GPS location (optional) ───────────────────────────────
            if (row.Latitude.HasValue && row.Longitude.HasValue)
            {
                var locResult = GeoLocation.Create(row.Latitude.Value, row.Longitude.Value);
                if (!locResult.Failed)
                    attraction.SetLocation(locResult.Value);
                else
                    logger.LogWarning(
                        "{Seeder}: Invalid geo-location for attraction '{Name}': {Error} — location skipped.",
                        nameof(AttractionSeeder), row.NameEn, locResult.Error);
            }

            // ── Arabic localized content ────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(row.NameAr) && !string.IsNullOrWhiteSpace(row.DescriptionAr))
            {
                var arabicResult = attraction.AddLocalizedContent(
                    LanguageCode.Arabic,
                    row.NameAr,
                    row.DescriptionAr,
                    row.LocationGuidAr);

                if (arabicResult.Failed)
                    logger.LogWarning(
                        "{Seeder}: Could not add Arabic content for attraction '{Name}': {Error}",
                        nameof(AttractionSeeder), row.NameEn, arabicResult.Error);
            }

            await dbContext.Attractions.AddAsync(attraction, cancellationToken);
            addedCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Seeder}: Committed {Count} attraction(s).", nameof(AttractionSeeder), addedCount);
    }
}
