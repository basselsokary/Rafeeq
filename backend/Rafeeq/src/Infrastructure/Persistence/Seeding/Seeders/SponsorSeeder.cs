using Domain.ValueObjects;
using Domain.Entities.SponsorAggregate;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Application.Common.Interfaces.Services;
using Infrastructure.Persistence.ApplicationContext;
using Application.Commands.Sponsors;
using System.Globalization;

namespace Infrastructure.Persistence.Seeding.Seeders;

/// <summary>
/// Seeds <see cref="Sponsor"/> aggregates from
/// <c>Persistence/Seeding/Data/Sponsors.csv</c> (embedded resource).
///
/// This seeder runs before <see cref="OfferSeeder"/> (order 70) so that
/// sponsors are available for FK resolution.
///
/// Idempotency: a sponsor is skipped when an English localized-content row with
/// the same title already exists in the database.
/// </summary>
internal sealed class SponsorSeeder(
    ApplicationDbContext dbContext,
    ICsvFileParser csvParser,
    ILogger<SponsorSeeder> logger) : IDataSeeder
{
    public int Order => 65;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = EmbeddedResourceHelper.GetCsvStream("Sponsors.csv");
        var rows = csvParser.ParseCsv<SponsorCsvRowDto>(stream);

        if (rows.Count == 0)
        {
            logger.LogWarning("{Seeder}: Sponsors.csv is empty — nothing to seed.", nameof(SponsorSeeder));
            return;
        }

        // Idempotency: skip sponsors whose English title already exists.
        var existingTitles = await dbContext.Sponsors
            .SelectMany(s => s.LocalizedContents)
            .Where(lc => lc.Language == LanguageCode.English)
            .Select(lc => lc.Title)
            .ToHashSetAsync(StringComparer.OrdinalIgnoreCase, cancellationToken);

        var toSeed = rows
            .Where(r => !existingTitles.Contains(r.TitleEn.Trim()))
            .ToList();

        if (toSeed.Count == 0)
        {
            logger.LogInformation("{Seeder}: All sponsors already seeded.", nameof(SponsorSeeder));
            return;
        }

        logger.LogInformation("{Seeder}: Seeding {Count} new sponsor(s).", nameof(SponsorSeeder), toSeed.Count);

        int seededCount = 0;

        foreach (var row in toSeed)
        {
            // ── Enums ───────────────────────────────────────────────────────
            if (!Enum.TryParse<SponsorType>(row.Type, ignoreCase: true, out var sponsorType))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping sponsor '{Title}' — unknown SponsorType '{Type}'.",
                    nameof(SponsorSeeder), row.TitleEn, row.Type);
                continue;
            }

            if (!Enum.TryParse<SponsorTier>(row.Tier, ignoreCase: true, out var sponsorTier))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping sponsor '{Title}' — unknown SponsorTier '{Tier}'.",
                    nameof(SponsorSeeder), row.TitleEn, row.Tier);
                continue;
            }

            if (!Enum.TryParse<SponsorStatus>(row.Status, ignoreCase: true, out var sponsorStatus))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping sponsor '{Title}' — unknown SponsorStatus '{Status}'.",
                    nameof(SponsorSeeder), row.TitleEn, row.Status);
                continue;
            }

            // ── Value objects ───────────────────────────────────────────────
            var locationResult = GeoLocation.Create(row.Latitude, row.Longitude);
            if (locationResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping sponsor '{Title}' — invalid geo-location: {Error}",
                    nameof(SponsorSeeder), row.TitleEn, locationResult.Error);
                continue;
            }

            var addressEnResult = Address.Create(row.AddressEn);
            if (addressEnResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping sponsor '{Title}' — invalid English address: {Error}",
                    nameof(SponsorSeeder), row.TitleEn, addressEnResult.Error);
                continue;
            }

            // ── Contract date range ─────────────────────────────────────────
            if (!DateTime.TryParseExact(
                row.StartDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var startDate) ||
            !DateTime.TryParseExact(
                row.EndDate,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var endDate))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping sponsor '{Title}' — cannot parse dates '{Start}' / '{End}'.",
                    nameof(SponsorSeeder), row.TitleEn, row.StartDate, row.EndDate);
                continue;
            }

            var dateRangeResult = DateRange.Create(startDate, endDate);
            if (dateRangeResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping sponsor '{Title}' — invalid contract date range: {Error}",
                    nameof(SponsorSeeder), row.TitleEn, dateRangeResult.Error);
                continue;
            }

            // ── Optional contact value objects ──────────────────────────────
            PhoneNumber? contactPhone = null;
            if (!string.IsNullOrWhiteSpace(row.ContactPhone))
            {
                var phoneResult = PhoneNumber.Create(row.ContactPhone);
                if (phoneResult.Failed)
                    logger.LogWarning(
                        "{Seeder}: Invalid phone '{Phone}' for sponsor '{Title}' — saved without phone.",
                        nameof(SponsorSeeder), row.ContactPhone, row.TitleEn);
                else
                    contactPhone = phoneResult.Value;
            }

            Email? contactEmail = null;
            if (!string.IsNullOrWhiteSpace(row.ContactEmail))
            {
                var emailResult = Email.Create(row.ContactEmail);
                if (emailResult.Failed)
                    logger.LogWarning(
                        "{Seeder}: Invalid email '{Email}' for sponsor '{Title}' — saved without email.",
                        nameof(SponsorSeeder), row.ContactEmail, row.TitleEn);
                else
                    contactEmail = emailResult.Value;
            }

            // ── Create sponsor aggregate ────────────────────────────────────
            var sponsorResult = Sponsor.Create(
                title: row.TitleEn,
                description: row.DescriptionEn,
                address: addressEnResult.Value,
                type: sponsorType,
                tier: sponsorTier,
                location: locationResult.Value,
                dateRange: dateRangeResult.Value,
                websiteUrl: row.WebsiteUrl,
                contactPhone: contactPhone,
                contactEmail: contactEmail);

            if (sponsorResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping sponsor '{Title}' — domain validation failed: {Error}",
                    nameof(SponsorSeeder), row.TitleEn, sponsorResult.Error);
                continue;
            }

            var sponsor = sponsorResult.Value;

            // ── Status ──────────────────────────────────────────────────────
            // Domain always initialises to Inactive. Only activate when CSV says Active
            // and the contract period is still valid (Activate() enforces this).
            if (sponsorStatus == SponsorStatus.Active)
            {
                var activateResult = sponsor.Activate();
                if (activateResult.Failed)
                    logger.LogWarning(
                        "{Seeder}: Could not activate sponsor '{Title}': {Error} — saved as Inactive.",
                        nameof(SponsorSeeder), row.TitleEn, activateResult.Error);
            }
            else if (sponsorStatus == SponsorStatus.Expired)
            {
                // Expired is a terminal state that the domain may not expose directly.
                // Deactivate as the safe fallback; the nightly job will mark it expired.
                sponsor.Deactivate();
            }

            // ── Arabic localized content ────────────────────────────────────
            if (!string.IsNullOrWhiteSpace(row.TitleAr) && !string.IsNullOrWhiteSpace(row.DescriptionAr))
            {
                var addressArResult = Address.Create(
                    string.IsNullOrWhiteSpace(row.AddressAr) ? row.AddressEn : row.AddressAr);

                if (!addressArResult.Failed)
                    sponsor.AddLocalizedContent(LanguageCode.Arabic, row.TitleAr, row.DescriptionAr, addressArResult.Value);
                else
                    logger.LogWarning(
                        "{Seeder}: Invalid Arabic address for sponsor '{Title}' — Arabic content skipped.",
                        nameof(SponsorSeeder), row.TitleEn);
            }

            await dbContext.Sponsors.AddAsync(sponsor, cancellationToken);
            seededCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Seeder}: Committed {Count} sponsor(s).", nameof(SponsorSeeder), seededCount);
    }
}
