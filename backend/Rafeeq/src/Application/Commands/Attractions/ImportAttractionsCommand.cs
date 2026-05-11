using Application.Commands.Attractions.Validators;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.DTOs.Common;
using Application.DTOs.Sites;
using Domain.Common.Interfaces;
using Domain.Entities.AttractionAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Attractions;

public record ImportAttractionsCommand(Stream CsvFile, string FileName, bool DryRun = true)
    : ICommand<ImportAttractionsResultDto>;

public record ImportAttractionsResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<AttractionRowErrorDto> Errors);

public record AttractionRowErrorDto(
    int RowNumber,
    string LogicalAttractionId,   // e.g. "SITE-001" — helps the data team find the row in the sheet
    List<string> Errors);

public sealed class ImportAttractionsHandler(
    IUnitOfWork unitOfWork,
    ISiteQueryService siteQueryService,
    ICsvFileParser csvParser) : ICommandHandler<ImportAttractionsCommand, ImportAttractionsResultDto>
{
    private readonly AttractionCsvRowValidator _rowValidator = new();

    public async Task<Result<ImportAttractionsResultDto>> HandleAsync(ImportAttractionsCommand command, CancellationToken ct)
    {
        List<AttractionCsvRowDto> rows;
        try
        {
            rows = csvParser.ParseCsv<AttractionCsvRowDto>(command.CsvFile);
        }
        catch (Exception ex)
        {
            return Result.Failure<ImportAttractionsResultDto>(
                ImportErrors.CsvParsingFailed(ex.Message));
        }

        // Load all sites into a name→Guid dictionary (one DB round-trip)
        var sites = await siteQueryService.GetAsync(
            filters: new SiteFilters(null, null, null, null, null),
            paging: new PagingParameters(PageNumber: 1, PageSize: int.MaxValue),
            cancellationToken: ct);

        var siteMap = sites.Data.ToDictionary(
            c => c.Name,
            c => c.Id,
            StringComparer.OrdinalIgnoreCase);

        var errors = new List<AttractionRowErrorDto>();
        var validAttractions = new List<Attraction>();

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
                rowErrors.Add($"Site not found in database: '{row.SiteName}'. Check the attraction name spelling in the sheet.");

            if (rowErrors.Count > 0)
            {
                errors.Add(new AttractionRowErrorDto(rowNumber, row.AttractionId, rowErrors));
                continue;
            }
            
            var attractionResult = MapToDomain(row, siteId);
            if (attractionResult.Failed)
            {
                errors.Add(new AttractionRowErrorDto(rowNumber, row.AttractionId, [attractionResult.Error.Message]));
                continue;
            }

            validAttractions.Add(attractionResult.Value);
        }

        // Batch insert — only valid rows reach here
        if (validAttractions.Count == rows.Count)
        {
            await unitOfWork.Attractions.AddRangeAsync(validAttractions, ct);
        }
        else
        {
            // Failed: don't save any rows if there are errors, to avoid partial imports and simplify error correction for the data team
            return new ImportAttractionsResultDto(
                TotalRows: rows.Count,
                SuccessCount: 0,
                FailureCount: errors.Count,
                Errors: errors);
        }

        // if (validAttractions.Count > 0)
        // {
        //     await unitOfWork.Attractions.AddRangeAsync(validAttractions, ct);
        // }

        if (!command.DryRun)
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Result.Success(new ImportAttractionsResultDto(
            TotalRows: rows.Count,
            SuccessCount: validAttractions.Count,
            FailureCount: errors.Count,
            Errors: errors));
    }

    private static Result<Attraction> MapToDomain(AttractionCsvRowDto row, Guid siteId)
    {
        // Enums — already validated, so Parse is safe
        var type   = Enum.Parse<AttractionType>(row.Type, ignoreCase: true);

        // --- Historical Periods ---
        List<HistoricalPeriod> historicalPeriods = new();
        if (!string.IsNullOrWhiteSpace(row.HistoricalPeriods))
        {
            historicalPeriods = row.HistoricalPeriods
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(f => Enum.Parse<HistoricalPeriod>(f, ignoreCase: true))
                .ToList();
        }

        // Nullable fields
        string? locationGuidEn = NullIfBlank(row.LocationGuidEn);
        string? locationGuidAr = NullIfBlank(row.LocationGuidAr);

        // --- Create the attraction (English content baked in via Attraction.Create) ---
        var attractionResult = Attraction.Create(
            siteId:                   siteId,
            name:                     row.NameEn,
            description:              row.DescriptionEn,
            locationDescription:      locationGuidEn,
            type:                     type,
            historicalPeriods:        historicalPeriods);

        if (attractionResult.Failed) return attractionResult;
        var attraction = attractionResult.Value;

        // GeoLocation value object
        bool latParsed = double.TryParse(row.Latitude, out var lat);
        bool lonParsed = double.TryParse(row.Longitude, out var lon);
        GeoLocation? location = null;
        if (latParsed && lonParsed)
        {
            var locationResult = GeoLocation.Create(lat, lon);
            if (locationResult.Succeeded)
                location = locationResult.Value;
            
            attraction.SetLocation(location);
        }

        // --- Arabic localized content ---
        var arResult = attraction.AddLocalizedContent(
            LanguageCode.Arabic,
            row.NameAr,
            row.DescriptionAr,
            locationGuidAr);
        if (arResult.Failed) return arResult.To<Attraction>();

        attraction.SetAsFeatured(row.IsFeatured);

        return attraction;
    }

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public sealed record AttractionCsvRowDto
{
    // Logical ID — used only in error messages, never stored in DB
    public string AttractionId { get; init; } = null!;

    // Site English name — resolved to Guid via DB lookup
    public string SiteName { get; init; } = null!;

    public string NameEn { get; init; } = null!;
    public string NameAr { get; init; } = null!;

    public string DescriptionEn { get; init; } = null!;
    public string DescriptionAr { get; init; } = null!;

    public string LocationGuidEn { get; init; } = null!;
    public string LocationGuidAr { get; init; } = null!;

    public string Type { get; init; } = null!;      // AttractionType enum

    public string? Latitude { get; init; }
    public string? Longitude { get; init; }

    public bool IsFeatured { get; init; }

    public string HistoricalPeriods { get; init; } = string.Empty;
}

