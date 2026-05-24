using Application.Commands.Artifacts;
using Application.Common.Interfaces.Services;
using Domain.Entities.ArtifactAggregate;
using Domain.Enums;
using Infrastructure.Persistence.ApplicationContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Persistence.Seeding.Seeders;

/// <summary>
/// Seeds <see cref="Artifact"/> aggregates from
/// <c>Persistence/Seeding/Data/Artifacts.csv</c> (embedded resource).
///
/// Depends on <see cref="SiteSeeder"/> (order 20) having already run.
///
/// Idempotency: an artifact is skipped when an English localized-content row
/// with the same name already exists for that site (or globally when SiteName is empty).
/// </summary>
internal sealed class ArtifactSeeder(
    ApplicationDbContext dbContext,
    ICsvFileParser csvParser,
    ILogger<ArtifactSeeder> logger) : IDataSeeder
{
    public int Order => 60;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = EmbeddedResourceHelper.GetCsvStream("Artifacts.csv");
        var rows = csvParser.ParseCsv<ArtifactCsvRowDto>(stream);

        if (rows.Count == 0)
        {
            logger.LogWarning("{Seeder}: Artifacts.csv is empty — nothing to seed.", nameof(ArtifactSeeder));
            return;
        }

        // Site name → Site ID lookup (English name as key).
        // Artifacts may have a null SiteId (standalone/museum-level artifact).
        var sites = await dbContext.Sites
            .AsSplitQuery()
            .Include(s => s.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .ToListAsync(cancellationToken);

        var siteIdByEnName = sites
            .Select(s => new
            {
                SiteId = s.Id,
                EnName = s.LocalizedContents
                    .FirstOrDefault()?.Name
            })
            .Where(x => x.EnName is not null)
            .ToDictionary(x => x.EnName!, x => x.SiteId, StringComparer.OrdinalIgnoreCase);

        // Idempotency: existing artifact English names (per site, null-site keyed by Guid.Empty).
        var existingArtifacts = await dbContext.Artifacts
            .Include(a => a.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .SelectMany(a => a.LocalizedContents
                .Select(lc => new { SiteId = a.SiteId ?? Guid.Empty, lc.Name }))
            .ToListAsync(cancellationToken);

        var existingSet = existingArtifacts
            .Select(x => (x.SiteId, x.Name.Trim().ToUpperInvariant()))
            .ToHashSet();

        int addedCount = 0;

        foreach (var row in rows)
        {
            // ── Site FK resolution (nullable) ───────────────────────────────
            Guid? siteId = null;

            if (!string.IsNullOrWhiteSpace(row.SiteName))
            {
                if (!siteIdByEnName.TryGetValue(row.SiteName.Trim(), out var resolvedSiteId))
                {
                    if (!row.SiteName.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
                    {   
                        logger.LogWarning(
                            "{Seeder}: Skipping artifact '{Name}' — site '{Site}' not found.",
                            nameof(ArtifactSeeder), row.NameEn, row.SiteName);
                        continue;
                    }
                }

                siteId = resolvedSiteId == default? null : resolvedSiteId;
            }

            // ── Idempotency ─────────────────────────────────────────────────
            var idempotencyKey = (siteId ?? Guid.Empty, row.NameEn.Trim().ToUpperInvariant());
            if (existingSet.Contains(idempotencyKey))
                continue;

            // ── ArtifactType ────────────────────────────────────────────────
            if (!Enum.TryParse<ArtifactType>(row.Type, ignoreCase: true, out var artifactType))
            {
                logger.LogWarning(
                    "{Seeder}: Unknown ArtifactType '{Type}' for artifact '{Name}' — skipped.",
                    nameof(ArtifactSeeder), row.Type, row.NameEn);
                continue;
            }

            // ── Create artifact ─────────────────────────────────────────────
            var artifactResult = Artifact.Create(
                siteId: siteId,
                name: row.NameEn,
                description: row.DescriptionEn,
                displayOrder: row.DisplayOrder,
                type: artifactType);

            if (artifactResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping artifact '{Name}' — domain validation failed: {Error}",
                    nameof(ArtifactSeeder), row.NameEn, artifactResult.Error);
                continue;
            }

            var artifact = artifactResult.Value;

            // ── Arabic localized content ────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(row.NameAr) && !string.IsNullOrWhiteSpace(row.DescriptionAr))
            {
                var arabicResult = artifact.AddLocalizedContent(
                    LanguageCode.Arabic,
                    row.NameAr,
                    row.DescriptionAr);

                if (arabicResult.Failed)
                    logger.LogWarning(
                        "{Seeder}: Could not add Arabic content for artifact '{Name}': {Error}",
                        nameof(ArtifactSeeder), row.NameEn, arabicResult.Error);
            }

            await dbContext.Artifacts.AddAsync(artifact, cancellationToken);
            addedCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Seeder}: Committed {Count} artifact(s).", nameof(ArtifactSeeder), addedCount);
    }
}
