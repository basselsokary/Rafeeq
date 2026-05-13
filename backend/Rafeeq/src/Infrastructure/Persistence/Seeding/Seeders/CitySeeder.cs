using Application.Commands.Cities;
using Application.Common.Interfaces.Services;
using Domain.Entities.CityAggregate;
using Domain.ValueObjects;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeding.Seeders;

/// <summary>
/// Seeds <see cref="City"/> and <see cref="CityLocalizedContent"/> records from
/// <c>Persistence/Seeding/Data/Cities.csv</c> (embedded resource).
///
/// Idempotency: a city is skipped when an English localized-content row with the
/// same name already exists in the database.
/// </summary>
internal sealed class CitySeeder(
    ApplicationDbContext dbContext,
    ICsvFileParser csvParser,
    ILogger<CitySeeder> logger) : IDataSeeder
{
    public int Order => 10; // Must run before SiteSeeder (order 20)

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = EmbeddedResourceHelper.GetCsvStream("Cities.csv");
        var rows = csvParser.ParseCsv<CityCsvRowDto>(stream);

        if (rows.Count == 0)
        {
            logger.LogWarning("{Seeder}: Cities.csv is empty — nothing to seed.", nameof(CitySeeder));
            return;
        }

        // Load all existing English city names for idempotency check.
        var existingNames = await dbContext.Cities
            .SelectMany(c => c.LocalizedContents)
            .Where(lc => lc.Language == LanguageCode.English)
            .Select(lc => lc.Name)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, cancellationToken);

        var toSeed = rows
            .Where(r => !existingNames.Contains(r.NameEn))
            .ToList();

        if (toSeed.Count == 0)
        {
            logger.LogInformation("{Seeder}: All cities already seeded.", nameof(CitySeeder));
            return;
        }

        logger.LogInformation("{Seeder}: Seeding {Count} new city/cities.", nameof(CitySeeder), toSeed.Count);

        foreach (var row in toSeed)
        {
            var locationResult = GeoLocation.Create(row.Latitude, row.Longitude);
            if (locationResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping city '{Name}' — invalid geo-location ({Lat},{Lng}): {Error}",
                    nameof(CitySeeder), row.NameEn, row.Latitude, row.Longitude, locationResult.Error);
                continue;
            }

            var cityResult = City.Create(
                name: row.NameEn,
                description: row.DescriptionEn,
                centerLocation: locationResult.Value,
                displayOrder: row.DisplayOrder);

            if (cityResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping city '{Name}' — domain validation failed: {Error}",
                    nameof(CitySeeder), row.NameEn, cityResult.Error);
                continue;
            }

            var city = cityResult.Value;

            // Add Arabic localized content when provided.
            if (!string.IsNullOrWhiteSpace(row.NameAr) && !string.IsNullOrWhiteSpace(row.DescriptionAr))
            {
                var arabicResult = city.AddLocalizedContent(LanguageCode.Arabic, row.NameAr, row.DescriptionAr);
                if (arabicResult.Failed)
                {
                    logger.LogWarning(
                        "{Seeder}: Could not add Arabic content for city '{Name}': {Error}",
                        nameof(CitySeeder), row.NameEn, arabicResult.Error);
                }
            }

            await dbContext.Cities.AddAsync(city, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Seeder}: Committed {Count} city/cities.", nameof(CitySeeder), toSeed.Count);
    }
}
