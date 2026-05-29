using Domain.ValueObjects;
using Domain.Entities.SponsorAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Infrastructure.Persistence.ApplicationContext;
using Application.Common.Interfaces.Services;
using Application.Commands.Sponsors.Offers;
using Domain.Enums;
using Infrastructure.Persistence.Seeding.Parsers;
using System.Globalization;

namespace Infrastructure.Persistence.Seeding.Seeders;

/// <summary>
/// Seeds <see cref="Offer"/> entities via <see cref="Sponsor.AddOffer"/> from
/// <c>Persistence/Seeding/Data/Offers.csv</c> (embedded resource).
///
/// Depends on <see cref="SponsorSeeder"/> (order 65) having already run.
///
/// Idempotency: an offer is skipped when an English localized-content row with the
/// same title already exists on the resolved sponsor.
/// </summary>
internal sealed class OfferSeeder(
    ApplicationDbContext dbContext,
    ICsvFileParser csvParser,
    ILogger<OfferSeeder> logger) : IDataSeeder
{
    public int Order => 70;

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var stream = EmbeddedResourceHelper.GetCsvStream("Offers.csv");
        var rows = csvParser.ParseCsv<OfferCsvRowDto>(stream);

        if (rows.Count == 0)
        {
            logger.LogWarning("{Seeder}: Offers.csv is empty — nothing to seed.", nameof(OfferSeeder));
            return;
        }

        // Load sponsors with their offers and localized contents for FK lookup & idempotency.
        var sponsors = await dbContext.Sponsors
            .AsSplitQuery()
            .Include(s => s.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .Include(s => s.Offers)
                .ThenInclude(o => o.LocalizedContents.Where(lc => lc.Language == LanguageCode.English))
            .ToListAsync(cancellationToken);

        var sponsorByEnTitle = sponsors
            .Select(s => new
            {
                Sponsor = s,
                EnTitle = s.LocalizedContents
                    .FirstOrDefault()?.Title
            })
            .Where(x => x.EnTitle is not null)
            .ToDictionary(x => x.EnTitle!, x => x.Sponsor, StringComparer.OrdinalIgnoreCase);

        int seededCount = 0;

        foreach (var row in rows)
        {
            // ── Sponsor FK resolution ───────────────────────────────────────
            if (!sponsorByEnTitle.TryGetValue(row.SponsorName.Trim(), out var sponsor))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping offer '{Title}' — sponsor '{Sponsor}' not found.",
                    nameof(OfferSeeder), row.TitleEn, row.SponsorName);
                continue;
            }

            // ── Idempotency ─────────────────────────────────────────────────
            bool alreadyExists = sponsor.Offers
                .Any(o => o.LocalizedContents
                    .Any(lc => string.Equals(lc.Title, row.TitleEn, StringComparison.OrdinalIgnoreCase)));

            if (alreadyExists)
                continue;

            // ── Discount Amount (nullable) ──────────────────────────────────
            Money? discountAmount = null;
            if (!string.IsNullOrWhiteSpace(row.DiscountAmount))
            {
                var amountResult = DiscountAmountParser.TryParse(row.DiscountAmount);
                if (amountResult is null || amountResult.Failed)
                {
                    logger.LogWarning(
                        "{Seeder}: Skipping offer '{Title}' — cannot parse discount amount '{Raw}': {Error}",
                        nameof(OfferSeeder), row.TitleEn, row.DiscountAmount,
                        amountResult?.Error.ToString() ?? "empty result");
                    continue;
                }
                discountAmount = amountResult.Value;
            }

            // ── Discount Percentage (nullable) ──────────────────────────────
            int? discountPercentage = null;
            if (!string.IsNullOrWhiteSpace(row.DiscountPercentage))
            {
                if (!DiscountPercentageParser(row.DiscountPercentage, out var pct))
                {
                    logger.LogWarning(
                        "{Seeder}: Skipping offer '{Title}' — cannot parse discount percentage '{Raw}'.",
                        nameof(OfferSeeder), row.TitleEn, row.DiscountPercentage);
                    continue;
                }
                discountPercentage = pct;
            }

            // Domain requires at least one discount to be present.
            if (discountAmount is null && discountPercentage is null)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping offer '{Title}' — both Discount Amount and Discount Percentage are empty.",
                    nameof(OfferSeeder), row.TitleEn);
                continue;
            }

            // ── Validity period ─────────────────────────────────────────────
            if (!DateTime.TryParseExact(
                row.StartValidityPeriod,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var startDate) ||
            !DateTime.TryParseExact(
                row.EndValidityPeriod,
                "dd/MM/yyyy",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var endDate))
            {
                logger.LogWarning(
                    "{Seeder}: Skipping offer '{Title}' — cannot parse validity period '{Start}' / '{End}'.",
                    nameof(OfferSeeder), row.TitleEn, row.StartValidityPeriod, row.EndValidityPeriod);
                continue;
            }

            var dateRangeResult = DateRange.Create(startDate, endDate);
            if (dateRangeResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Skipping offer '{Title}' — invalid validity period: {Error}",
                    nameof(OfferSeeder), row.TitleEn, dateRangeResult.Error);
                continue;
            }

            // ── Optional fields ─────────────────────────────────────────────
            int? maxRedemptions = null;
            if (!string.IsNullOrWhiteSpace(row.MaxRedemptions))
            {
                if (!int.TryParse(row.MaxRedemptions, out var maxR))
                {
                    logger.LogWarning(
                        "{Seeder}: Cannot parse MaxRedemptions '{Raw}' for offer '{Title}' — saved without limit.",
                        nameof(OfferSeeder), row.MaxRedemptions, row.TitleEn);
                }
                else
                {
                    maxRedemptions = maxR;
                }
            }

            string? promoCode = string.IsNullOrWhiteSpace(row.PromoCode) ? null : row.PromoCode.Trim();

            // ── Add offer to sponsor aggregate ──────────────────────────────
            var offerResult = sponsor.AddOffer(
                discount: discountAmount,
                discountPercentage: discountPercentage,
                validityPeriod: dateRangeResult.Value,
                maxRedemptions: maxRedemptions,
                promoCode: promoCode);

            if (offerResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Could not add offer '{Title}' to sponsor '{Sponsor}': {Error}",
                    nameof(OfferSeeder), row.TitleEn, row.SponsorName, offerResult.Error);
                continue;
            }

            var offer = offerResult.Value;

            // ── English localized content ───────────────────────────────────
            var enContentResult = offer.AddLocalizedContent(
                LanguageCode.English,
                row.TitleEn,
                row.DescriptionEn,
                row.TermsAndConditionsEn);

            if (enContentResult.Failed)
            {
                logger.LogWarning(
                    "{Seeder}: Could not add English content for offer '{Title}': {Error} — offer skipped.",
                    nameof(OfferSeeder), row.TitleEn, enContentResult.Error);

                // Roll back the offer from the sponsor in-memory.
                sponsor.RemoveOffer(offer.Id);
                continue;
            }

            // ── Arabic localized content (optional) ─────────────────────────
            if (!string.IsNullOrWhiteSpace(row.TitleAr) && !string.IsNullOrWhiteSpace(row.DescriptionAr))
            {
                var arContentResult = offer.AddLocalizedContent(
                    LanguageCode.Arabic,
                    row.TitleAr,
                    row.DescriptionAr,
                    row.TermsAndConditionsAr);

                if (arContentResult.Failed)
                    logger.LogWarning(
                        "{Seeder}: Could not add Arabic content for offer '{Title}': {Error}",
                        nameof(OfferSeeder), row.TitleEn, arContentResult.Error);
            }

            offer.Activate();

            await dbContext.AddAsync(offer, cancellationToken);

            seededCount++;
        }

        await dbContext.SaveChangesAsync(cancellationToken);
        logger.LogInformation("{Seeder}: Committed {Count} offer(s).", nameof(OfferSeeder), seededCount);
    }

    private static bool DiscountPercentageParser(string value, out int result)
    {
        // Remove any trailing % sign before parsing.
        var cleaned = value.Trim().TrimEnd('%');
        bool isValid = int.TryParse(cleaned, out var percentage) && percentage >= 0 && percentage <= 100;
        result =  isValid ? percentage : 0;
        return isValid;
    }
}
