using Application.Commands.Sites.NearestTransportations.Validators;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites.NearestTransportations;

public record ImportNearestTransportationsCommand(Stream CsvFile, string FileName, bool DryRun = true)
    : ICommand<ImportNearestTransportationsResultDto>;

public record ImportNearestTransportationsResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<NearestTransportationRowErrorDto> Errors);

public record NearestTransportationRowErrorDto(
    int RowNumber,
    string LogicalTransportId,   // transport name (English) — helps data team find the row
    List<string> Errors);

public sealed class ImportNearestTransportationsHandler(
    IUnitOfWork unitOfWork,
    ISiteQueryService siteQueryService,
    ICsvFileParser csvParser)
    : ICommandHandler<ImportNearestTransportationsCommand, ImportNearestTransportationsResultDto>
{
    private readonly NearestTransportationCsvRowValidator _rowValidator = new();

    public async Task<Result<ImportNearestTransportationsResultDto>> HandleAsync(
        ImportNearestTransportationsCommand command,
        CancellationToken ct)
    {
        // ── 1. Parse CSV ──────────────────────────────────────────────────────
        List<NearestTransportationCsvRowDto> rows;
        try
        {
            rows = csvParser.ParseCsv<NearestTransportationCsvRowDto>(command.CsvFile);
        }
        catch (Exception ex)
        {
            return Result.Failure<ImportNearestTransportationsResultDto>(
                ImportErrors.CsvParsingFailed(ex.Message));
        }

        // ── 2. Load all sites (one DB round-trip) ─────────────────────────────
        var sites = await siteQueryService.GetAsync(
            filters: new DTOs.Sites.SiteFilters(null, null, null, null, null),
            paging: new DTOs.Common.PagingParameters(PageNumber: 1, PageSize: int.MaxValue),
            cancellationToken: ct);

        var siteMap = sites.Data.ToDictionary(
            s => s.Name,
            s => s.Id,
            StringComparer.OrdinalIgnoreCase);

        // ── 3. Validate + map each row ────────────────────────────────────────
        var errors = new List<NearestTransportationRowErrorDto>();
        var validTransportations = new List<NearestTransportation>();

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            int rowNumber = i + 2; // +2: row 1 is the header
            var rowErrors = new List<string>();

            // --- FluentValidation ---
            var validation = await _rowValidator.ValidateAsync(row, ct);
            if (!validation.IsValid)
                rowErrors.AddRange(validation.Errors.Select(e => e.ErrorMessage));

            // --- Site resolution ---
            if (!siteMap.TryGetValue(row.SiteName, out var siteId))
                rowErrors.Add($"Site not found in database: '{row.SiteName}'. Check the site name spelling in the sheet.");

            if (rowErrors.Count > 0)
            {
                errors.Add(new NearestTransportationRowErrorDto(rowNumber, row.NameEn, rowErrors));
                continue;
            }

            var transportResult = MapToDomain(row, siteId);
            if (transportResult.Failed)
            {
                errors.Add(new NearestTransportationRowErrorDto(
                    rowNumber, row.NameEn, [transportResult.Error.Message]));
                continue;
            }

            validTransportations.Add(transportResult.Value);
        }

        // ── 4. All-or-nothing: only persist if every row is valid ─────────────
        if (validTransportations.Count != rows.Count)
        {
            return new ImportNearestTransportationsResultDto(
                TotalRows: rows.Count,
                SuccessCount: 0,
                FailureCount: errors.Count,
                Errors: errors);
        }

        await unitOfWork.Sites.AddNearestTransportationsRangeAsync(validTransportations, ct);

        if (!command.DryRun)
            await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new ImportNearestTransportationsResultDto(
            TotalRows: rows.Count,
            SuccessCount: validTransportations.Count,
            FailureCount: errors.Count,
            Errors: errors));
    }

    // ── Domain mapping ────────────────────────────────────────────────────────

    private static Result<NearestTransportation> MapToDomain(
        NearestTransportationCsvRowDto row,
        Guid siteId)
    {
        var type = Enum.Parse<TransportationType>(row.Type, ignoreCase: true);

        var locationResult = GeoLocation.Create(row.Latitude, row.Longitude);
        if (locationResult.Failed)  return locationResult.Error;

        // --- Create the entity ---
        var createResult = NearestTransportation.Create(siteId, type, locationResult.Value, row.DistanceKm);
        if (createResult.Failed) return createResult;

        var transportation = createResult.Value;

        // --- Optional flags / hours ---
        transportation.SetOperationalStatus(row.IsOperational);
        transportation.SetAccessibility(row.HasAccessibility);

        if (!string.IsNullOrWhiteSpace(row.OperatingHours))
        {
            var timeRange = ParseTimeRange(row.OperatingHours);
            if (timeRange is not null)
                transportation.SetOperatingHours(timeRange);
        }

        // --- English localized content ---
        var enAddress = BuildAddress(row.AddressEn);
        var enResult = transportation.AddLocalizedContent(
            LanguageCode.English,
            row.NameEn,
            NullIfBlank(row.DescriptionEn),
            enAddress);
        if (enResult.Failed) return enResult.To<NearestTransportation>();

        // --- Arabic localized content ---
        var arAddress = BuildAddress(row.AddressAr);
        var arResult = transportation.AddLocalizedContent(
            LanguageCode.Arabic,
            row.NameAr,
            NullIfBlank(row.DescriptionAr),
            arAddress);
        if (arResult.Failed) return arResult.To<NearestTransportation>();

        return transportation;
    }

    /// <summary>
    /// Parses "05:00 AM - 01:00 AM" into a <see cref="TimeRange"/>.
    /// Row validator already guarantees the format is valid when this is called.
    /// </summary>
    private static TimeRange? ParseTimeRange(string value)
    {
        var parts = value.Split('-', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2) return null;

        if (!TimeOnly.TryParse(parts[0], out var opens)) return null;
        if (!TimeOnly.TryParse(parts[1], out var closes)) return null;

        
        var result = TimeRange.Create(opens, closes);
        return result.Succeeded ? result.Value : null;
    }

    /// <summary>
    /// Builds an <see cref="Address"/> value object from a plain string, or null if blank.
    /// Adjust if Address has additional fields (city, country, etc.).
    /// </summary>
    private static Address? BuildAddress(string? raw)
    {
        var result = Address.Create(raw!);
        return result.Succeeded ? result.Value : null;
    }

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record NearestTransportationCsvRowDto
{
    // Logical ID — used only in error messages, never stored in DB
    public string TransportId { get; init; } = null!;

    // Site English name — resolved to Guid via DB lookup
    public string SiteName { get; init; } = null!;

    public string NameEn { get; init; } = null!;
    public string NameAr { get; init; } = null!;

    public string? DescriptionEn { get; init; }
    public string? DescriptionAr { get; init; }

    public string? AddressEn { get; init; }
    public string? AddressAr { get; init; }

    public string Type { get; init; } = null!;   // TransportationType enum

    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public double DistanceKm { get; init; }

    public bool IsOperational { get; init; }
    public bool HasAccessibility { get; init; }

    // Optional — e.g. "05:00 AM - 01:00 AM". Null/empty means no operating hours set.
    public string? OperatingHours { get; init; }
}