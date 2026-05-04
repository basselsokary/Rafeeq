using Application.Commands.Sites.Validators;
using CsvHelper;
using CsvHelper.Configuration;
using Domain.Common.Interfaces;
using Domain.Entities.SiteAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites;

public record ImportSitesCommand(Stream CsvFile, string FileName) : ICommand<ImportSitesResultDto>;

public record ImportSitesResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<SiteRowErrorDto> Errors);

public record SiteRowErrorDto(
    int RowNumber,
    string LogicalSiteId,   // e.g. "SITE-001" — helps the data team find the row in the sheet
    List<string> Errors);

public sealed class ImportSitesHandler(
    IUnitOfWork unitOfWork) : ICommandHandler<ImportSitesCommand, ImportSitesResultDto>
{
    private readonly SiteCsvRowValidator _rowValidator = new();

    public async Task<Result<ImportSitesResultDto>> HandleAsync(ImportSitesCommand command, CancellationToken ct)
    {
        List<SiteCsvRowDto> rows;
        try
        {
            rows = ParseCsv(command.CsvFile);
        }
        catch (Exception ex)
        {
            return Result.Failure<ImportSitesResultDto>(
                ImportErrors.CsvParsingFailed(ex.Message));
        }

        // Load all cities into a name→Guid dictionary (one DB round-trip)
        var cities = await unitOfWork.Cities.GetAllAsync(ct);
        var cityMap = cities.ToDictionary(
            c => c.LocalizedContents
                .First(lc => lc.Language == LanguageCode.English)
                .Name,
            c => c.Id,
            StringComparer.OrdinalIgnoreCase);

        var errors = new List<SiteRowErrorDto>();
        var validSites = new List<Site>();

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            int rowNumber = i + 2; // +2: row 1 is the header
            var rowErrors = new List<string>();

            // --- FluentValidation ---
            var validation = await _rowValidator.ValidateAsync(row, ct);
            if (!validation.IsValid)
                rowErrors.AddRange(validation.Errors.Select(e => e.ErrorMessage));

            // --- City resolution ---
            if (!cityMap.TryGetValue(row.CityName, out var cityId))
                rowErrors.Add($"City not found in database: '{row.CityName}'. Check the city name spelling in the sheet.");

            if (rowErrors.Count > 0)
            {
                errors.Add(new SiteRowErrorDto(rowNumber, row.SiteId, rowErrors));
                continue;
            }
            
            var siteResult = MapToDomain(row, cityId);
            if (siteResult.Failed)
            {
                errors.Add(new SiteRowErrorDto(rowNumber, row.SiteId, [siteResult.Error.Message]));
                continue;
            }

            validSites.Add(siteResult.Value);
        }

        // Batch insert — only valid rows reach here
        if (validSites.Count == rows.Count)
        {
            await unitOfWork.Sites.AddRangeAsync(validSites, ct);
            await unitOfWork.SaveChangesAsync(ct);
        }
        else
        {
            // Failed: don't save any rows if there are errors, to avoid partial imports and simplify error correction for the data team
            return new ImportSitesResultDto(
                TotalRows: rows.Count,
                SuccessCount: 0,
                FailureCount: errors.Count,
                Errors: errors);
        }

        // if (validSites.Count > 0)
        // {
        //     await unitOfWork.Sites.AddRangeAsync(validSites, ct);
        //     await unitOfWork.SaveChangesAsync(ct);
        // }

        return Result.Success(new ImportSitesResultDto(
            TotalRows: rows.Count,
            SuccessCount: validSites.Count,
            FailureCount: errors.Count,
            Errors: errors));
    }

    private static Result<Site> MapToDomain(SiteCsvRowDto row, Guid cityId)
    {
        // Enums — already validated, so Parse is safe
        var status = Enum.Parse<SiteStatus>(row.Status, ignoreCase: true);
        var type   = Enum.Parse<SiteType>(row.Type, ignoreCase: true);

        // GeoLocation value object
        var locationResult = GeoLocation.Create(row.Latitude, row.Longitude);
        if (locationResult.Failed) return locationResult.To<Site>();

        // Address value objects
        var addressEnResult = Address.Create(row.AddressEn);
        if (addressEnResult.Failed) return addressEnResult.To<Site>();

        var addressArResult = Address.Create(row.AddressAr);
        if (addressArResult.Failed) return addressArResult.To<Site>();

        // Nullable fields
        string? entryFeeNote  = NullIfBlank(row.EntryFeeNote);
        string? contactPhone  = NullIfBlank(row.ContactPhone);
        string? websiteUrl    = NullIfBlank(row.WebsiteUrl);

        // --- Create the site (English content baked in via Site.Create) ---
        var siteResult = Site.Create(
            cityId:                   cityId,
            name:                     row.NameEn,
            description:              row.DescriptionEn,
            location:                 locationResult.Value,
            entryFeeNotes:            entryFeeNote,
            address:                  addressEnResult.Value,
            type:                     type,
            estimatedDurationMinutes: row.EstimatedDurationMinutes,
            contactPhone:             contactPhone,
            contactWebsiteUrl:        websiteUrl
            );

        if (siteResult.Failed) return siteResult;
        var site = siteResult.Value;

        // --- Arabic localized content ---
        var arResult = site.AddLocalizedContent(
            LanguageCode.Arabic,
            row.NameAr,
            row.DescriptionAr,
            addressArResult.Value,
            entryFeeNote);
        if (arResult.Failed) return arResult.To<Site>();

        // --- Entry fee ---
        if (!string.IsNullOrWhiteSpace(row.EntryFee))
        {
            var parts      = row.EntryFee.Split(',');
            var foreignFee = decimal.Parse(parts[0].Trim());
            var egyptianFee = decimal.Parse(parts[1].Trim());

            // Ticket.Create(egyptianPrice, foreignerPrice)
            var egyptianMoneyResult = Money.Create(egyptianFee, "EGP");
            if (egyptianMoneyResult.Failed) return egyptianMoneyResult.To<Site>();

            var foreignMoneyResult  = Money.Create(foreignFee, "EGP");
            if (foreignMoneyResult.Failed) return foreignMoneyResult.To<Site>();

            var ticketResult = Ticket.Create(egyptianMoneyResult.Value, foreignMoneyResult.Value);
            if (ticketResult.Failed) return ticketResult.To<Site>();

            site.SetEntryFee(ticketResult.Value);
        }
        else
        {
            // Entry Fee is blank → either unknown (isFree=false) or actually free (isFree=true)
            site.RemoveEntryFee(row.IsFree);
        }

        // --- Featured / Hidden Gem ---
        if (row.IsFeatured)
        {
            var featuredResult = site.UpdateStatus(status, false, true);
            if (featuredResult.Failed) return featuredResult.To<Site>();
        }

        if (row.IsHiddenGem)
        {
            var hiddenGemResult = site.UpdateStatus(status, true, false);
            if (hiddenGemResult.Failed) return hiddenGemResult.To<Site>();
        }

        // --- Facilities ---
        if (!string.IsNullOrWhiteSpace(row.Facilities))
        {
            var facilities = row.Facilities
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(f => Enum.Parse<FacilityType>(f, ignoreCase: true))
                .ToList();

            var facilitiesResult = site.AddFacilities(facilities);
            if (facilitiesResult.Failed) return facilitiesResult.To<Site>();
        }

        return site;
    }

    private static List<SiteCsvRowDto> ParseCsv(Stream file)
    {
        using var reader = new StreamReader(file);
        using var csv    = new CsvReader(reader);
        csv.Configuration.RegisterClassMap<SiteCsvRowMap>();
        return csv.GetRecords<SiteCsvRowDto>().ToList();
    }

    private static string? NullIfBlank(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value.Trim();
}

public class ImportErrors
{
    public static Error CsvParsingFailed(string message)
        => Error.Failure("CSV_PARSING_FAILED", $"Failed to parse CSV file: {message}");
}

public sealed record SiteCsvRowDto
{
    // Logical ID — used only in error messages, never stored in DB
    public string SiteId { get; init; } = null!;

    // City English name — resolved to Guid via DB lookup
    public string CityName { get; init; } = null!;

    public string NameEn { get; init; } = null!;
    public string NameAr { get; init; } = null!;

    public string DescriptionEn { get; init; } = null!;
    public string DescriptionAr { get; init; } = null!;

    public string AddressEn { get; init; } = null!;
    public string AddressAr { get; init; } = null!;

    public string Status { get; init; } = null!;   // SiteStatus enum
    public string Type { get; init; } = null!;      // SiteType enum

    public double Latitude { get; init; }
    public double Longitude { get; init; }

    // "700,60" → ForeignerFee=700 EGP, EgyptianFee=60 EGP
    public string? EntryFee { get; init; }
    public string? EntryFeeNote { get; init; }
    public bool IsFree { get; init; }  // TRUE/FALSE

    public int EstimatedDurationMinutes { get; init; }

    public string? WebsiteUrl { get; init; }
    public string? ContactPhone { get; init; }

    public bool IsFeatured { get; init; }
    public bool IsHiddenGem { get; init; }

    // "Restrooms, Parking, CafeteriaOrRestaurant"
    public string Facilities { get; init; } = string.Empty;
}

public sealed class SiteCsvRowMap : CsvClassMap<SiteCsvRowDto>
{
    public SiteCsvRowMap()
    {
        Map(m => m.SiteId).Name("Site ID");
        Map(m => m.CityName).Name("Parent City ID");
        Map(m => m.NameEn).Name("Site Name (English)");
        Map(m => m.NameAr).Name("Site Name (Localized)");
        Map(m => m.DescriptionEn).Name("Description (English)");
        Map(m => m.DescriptionAr).Name("Description (Localized)");
        Map(m => m.AddressEn).Name("Address (English)");
        Map(m => m.AddressAr).Name("Address (Localized)");
        Map(m => m.Status).Name("Status");
        Map(m => m.Type).Name("Type");
        Map(m => m.Latitude).Name("Latitude");
        Map(m => m.Longitude).Name("Longitude");
        Map(m => m.EntryFee).Name("Entry Fee");
        Map(m => m.EntryFeeNote).Name("Entry Fee Note");
        Map(m => m.IsFree).Name("Is Free?");
        Map(m => m.EstimatedDurationMinutes).Name("Estimated Duration Minutes");
        Map(m => m.WebsiteUrl).Name("Website URL");
        Map(m => m.ContactPhone).Name("Contact Phone");
        Map(m => m.IsFeatured).Name("Is Featured?");
        Map(m => m.IsHiddenGem).Name("Is Hidden Gem?");
        Map(m => m.Facilities).Name("Facilities");
    }
}