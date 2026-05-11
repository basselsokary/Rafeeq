using Application.Commands.Cities.Validators;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Domain.Common.Interfaces;
using Domain.Entities.CityAggregate;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Cities;

public record ImportCitiesCommand(Stream CsvFile, string FileName, bool DryRun = true)
    : ICommand<ImportCitiesResultDto>;

public record ImportCitiesResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<CityRowErrorDto> Errors);

public record CityRowErrorDto(
    int RowNumber,
    string LogicalCityId,   // e.g. "SITE-001" — helps the data team find the row in the sheet
    List<string> Errors);

public sealed class ImportCitiesHandler(
    IUnitOfWork unitOfWork,
    ICsvFileParser csvParser) : ICommandHandler<ImportCitiesCommand, ImportCitiesResultDto>
{
    private readonly CityCsvRowValidator _rowValidator = new();

    public async Task<Result<ImportCitiesResultDto>> HandleAsync(ImportCitiesCommand command, CancellationToken ct)
    {
        List<CityCsvRowDto> rows;
        try
        {
            rows = csvParser.ParseCsv<CityCsvRowDto>(command.CsvFile);
        }
        catch (Exception ex)
        {
            return Result.Failure<ImportCitiesResultDto>(
                ImportErrors.CsvParsingFailed(ex.Message));
        }

        var errors = new List<CityRowErrorDto>();
        var validCities = new List<City>();

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            int rowNumber = i + 2; // +2: row 1 is the header
            var rowErrors = new List<string>();

            // --- FluentValidation ---
            var validation = await _rowValidator.ValidateAsync(row, ct);
            if (!validation.IsValid)
                rowErrors.AddRange(validation.Errors.Select(e => e.ErrorMessage));

            if (rowErrors.Count > 0)
            {
                errors.Add(new CityRowErrorDto(rowNumber, row.CityId, rowErrors));
                continue;
            }
            
            var cityResult = MapToDomain(row);
            if (cityResult.Failed)
            {
                errors.Add(new CityRowErrorDto(rowNumber, row.CityId, [cityResult.Error.Message]));
                continue;
            }

            validCities.Add(cityResult.Value);
        }

        // Batch insert — only valid rows reach here
        if (validCities.Count == rows.Count)
        {
            await unitOfWork.Cities.AddRangeAsync(validCities, ct);
        }
        else
        {
            // Failed: don't save any rows if there are errors, to avoid partial imports and simplify error correction for the data team
            return new ImportCitiesResultDto(
                TotalRows: rows.Count,
                SuccessCount: 0,
                FailureCount: errors.Count,
                Errors: errors);
        }

        // if (validCities.Count > 0)
        // {
        //     await unitOfWork.Cities.AddRangeAsync(validCities, ct);
        // }

        if (!command.DryRun)
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Result.Success(new ImportCitiesResultDto(
            TotalRows: rows.Count,
            SuccessCount: validCities.Count,
            FailureCount: errors.Count,
            Errors: errors));
    }

    private static Result<City> MapToDomain(CityCsvRowDto row)
    {
        // GeoLocation value object
        var locationResult = GeoLocation.Create(row.Latitude, row.Longitude);
        if (locationResult.Failed) return locationResult.To<City>();

        // --- Create the city (English content baked in via City.Create) ---
        var cityResult = City.Create(
            name:                     row.NameEn,
            description:              row.DescriptionEn,
            centerLocation:                 locationResult.Value,
            displayOrder:             row.DisplayOrder);

        if (cityResult.Failed) return cityResult;
        var city = cityResult.Value;

        // --- Arabic localized content ---
        var arResult = city.AddLocalizedContent(
            LanguageCode.Arabic,
            row.NameAr,
            row.DescriptionAr);
        if (arResult.Failed) return arResult.To<City>();

        return city;
    }
}



public sealed record CityCsvRowDto
{
    // Logical ID — used only in error messages, never stored in DB
    public string CityId { get; init; } = null!;

    public string NameEn { get; init; } = null!;
    public string NameAr { get; init; } = null!;

    public string DescriptionEn { get; init; } = null!;
    public string DescriptionAr { get; init; } = null!;

    public double Latitude { get; init; }
    public double Longitude { get; init; }

    public int DisplayOrder { get; init; }
}

