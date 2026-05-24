using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;

namespace Application.Commands.Sponsors.Offers;

public record ImportOffersCommand(Stream CsvFile, string FileName, bool DryRun = true)
    : ICommand<ImportOffersResultDto>;

public record ImportOffersResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<OfferRowErrorDto> Errors);

public record OfferRowErrorDto(
    int RowNumber,
    string LogicalOfferId,   // offer name (English) — helps data team find the row
    List<string> Errors);

public sealed class ImportOffersHandler(
    IUnitOfWork unitOfWork,
    ISponsorQueryService sponsorQueryService,
    ICsvFileParser csvParser)
    : ICommandHandler<ImportOffersCommand, ImportOffersResultDto>
{
    public async Task<Result<ImportOffersResultDto>> HandleAsync(
        ImportOffersCommand command,
        CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}

public sealed class OfferCsvRowDto
{
    public string OfferId { get; set; } = string.Empty;               // Stable human-readable seeding key (not a GUID)
    public string SponsorName { get; set; } = string.Empty;           // FK lookup — matches Title (English) in Sponsors.csv
    public string TitleEn { get; set; } = string.Empty;
    public string TitleAr { get; set; } = string.Empty;
    public string DescriptionEn { get; set; } = string.Empty;
    public string DescriptionAr { get; set; } = string.Empty;
    public string? TermsAndConditionsEn { get; set; }
    public string? TermsAndConditionsAr { get; set; }

    /// <summary>
    /// Raw discount amount cell — plain decimal or with currency symbol.
    /// Leave empty when only a percentage discount applies.
    /// Parsed via <see cref="DiscountAmountParser.TryParse"/>.
    /// </summary>
    public string? DiscountAmount { get; set; }

    /// <summary>
    /// Discount percentage as an integer string (0–100).
    /// Leave empty when only a flat amount discount applies.
    /// </summary>
    public string? DiscountPercentage { get; set; }

    public string StartValidityPeriod { get; set; } = string.Empty;   // yyyy-MM-dd
    public string EndValidityPeriod { get; set; } = string.Empty;     // yyyy-MM-dd
    public string? MaxRedemptions { get; set; }
    public string? PromoCode { get; set; }
}