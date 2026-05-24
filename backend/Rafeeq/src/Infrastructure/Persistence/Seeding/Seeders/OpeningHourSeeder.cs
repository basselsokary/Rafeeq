using Application.Commands.Sites.OpeningHours;
using Application.Common.Interfaces.Services;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeding.Seeders;

/// <summary>
/// Seeds <see cref="OpeningHour"/> value objects onto existing <see cref="Site"/> aggregates
/// from <c>Persistence/Seeding/Data/OpeningHours.csv</c> (embedded resource).
///
/// Depends on <see cref="SiteSeeder"/> (order 20) having already run.
///
/// Idempotency: for each site, a weekday entry is only added if that site does not
/// already have an opening-hour record for that day.
/// </summary>
internal sealed class OpeningHourSeeder(
    ApplicationDbContext dbContext,
    ICsvFileParser csvParser,
    ILogger<OpeningHourSeeder> logger) : IDataSeeder
{
    public int Order => 30;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = EmbeddedResourceHelper.GetCsvStream("OpeningHours.csv");
        var rows = csvParser.ParseCsv<OpeningHourCsvRowDto>(stream);

        if (rows.Count == 0)
        {
            logger.LogWarning("{Seeder}: OpeningHours.csv is empty — nothing to seed.", nameof(OpeningHourSeeder));
            return;
        }

        // Group rows by the stable Site ID defined in the CSV.
        var rowsBySiteId = rows.GroupBy(r => r.ParentSiteId).ToList();

        // Build a lookup: CSV site-id → database Site entity.
        // The seeder uses the English name as the lookup key when ParentSiteId
        // is a human-readable identifier (matches SiteId column of Sites.csv).
        // We resolve via SiteName as a fallback.
        var sites = await dbContext.Sites
            .AsSplitQuery()
            .Include(s => s.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .Include(s => s.OpeningHours)
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

        // All days of the week that can be covered by ALLDAYS.
        var allWeekDays = Enum.GetValues<WeekDay>();

        foreach (var group in rowsBySiteId)
        {
            // Resolve site by SiteName (first row in group is sufficient for lookup).
            var siteName = group.First().SiteName;
            if (!siteByEnName.TryGetValue(siteName.Trim(), out var site))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping opening hours for site '{Site}' — not found in database.",
                    nameof(OpeningHourSeeder), siteName);
                continue;
            }

            foreach (var row in group)
            {
                
                // ── ALLDAYS shorthand ────────────────────────────────────────
                // A row with Day = "ALLDAYS" applies the same schedule to every
                // weekday that does not already have an entry for this site.
                if (string.Equals(row.Day, "ALLDAYS", StringComparison.OrdinalIgnoreCase))
                {

                    foreach (var weekDay in allWeekDays)
                    {
                        if (site.OpeningHours.Any(oh => oh.Day == weekDay))
                            continue; // Specific entry already exists — don't overwrite.

                        var timeRangeForAll = BuildTimeRange(row, siteName);
                        
                        if (timeRangeForAll is null && !row.IsClosed)
                        {
                            logger.LogWarning(
                                "{Seeder}: ALLDAYS row for site '{Site}' has an invalid time range — day '{Day}' skipped.",
                                nameof(OpeningHourSeeder), siteName, weekDay);
                            continue;
                        }

                        site.AddOpeningHour(weekDay, timeRangeForAll, row.IsClosed);
                        addedCount++;
                    }
                    continue;
                }

                // ── Single weekday ───────────────────────────────────────────
                if (!Enum.TryParse<WeekDay>(row.Day, ignoreCase: true, out var singleDay))
                {
                    logger.LogWarning(
                        "{Seeder}: Unknown day '{Day}' for site '{Site}' — skipped.",
                        nameof(OpeningHourSeeder), row.Day, siteName);
                    continue;
                }

                // Idempotency: skip if this day already exists on the site.
                if (site.OpeningHours.Any(oh => oh.Day == singleDay))
                    continue;

                var timeRange = BuildTimeRange(row, siteName);
                if (timeRange is null && !row.IsClosed)
                    continue;

                site.AddOpeningHour(singleDay, timeRange, isClosed: false);
                addedCount++;
            }
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Seeder}: Committed {Count} opening hour(s).", nameof(OpeningHourSeeder), addedCount);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    /// <summary>
    /// Parses <see cref="OpeningHourCsvRowDto.StartTime"/> and <see cref="OpeningHourCsvRowDto.EndTime"/>
    /// into a <see cref="TimeRange"/>. Returns <c>null</c> and logs a warning on failure.
    /// Always returns <c>null</c> when the row is marked as closed (caller handles that path separately).
    /// </summary>
    private TimeRange? BuildTimeRange(OpeningHourCsvRowDto row, string siteName)
    {
        if (row.IsClosed)
            return null;

        if (!TimeOnly.TryParseExact(row.StartTime, "HH:mm", out var start) ||
            !TimeOnly.TryParseExact(row.EndTime, "HH:mm", out var end))
        {
            logger.LogWarning(
                "{Seeder}: Cannot parse time '{Start}'-'{End}' for site '{Site}', day '{Day}' — skipped.",
                nameof(OpeningHourSeeder), row.StartTime, row.EndTime, siteName, row.Day);
            return null;
        }

        var result = TimeRange.Create(start, end, differentDays: row.IsOvernight);
        if (result.Failed)
        {
            logger.LogWarning(
                "{Seeder}: Invalid time range for site '{Site}', day '{Day}': {Error}",
                nameof(OpeningHourSeeder), siteName, row.Day, result.Error);
            return null;
        }

        return result.Value;
    }
}
