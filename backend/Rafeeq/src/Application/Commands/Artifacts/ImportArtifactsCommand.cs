using Application.Commands.Artifacts.Validators;
using Application.Common.Interfaces.QueryServices;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Application.DTOs.Common;
using Application.DTOs.Sites;
using Domain.Common.Interfaces;
using Domain.Entities.ArtifactAggregate;
using Domain.Enums;

namespace Application.Commands.Artifacts;

public record ImportArtifactsCommand(Stream CsvFile, string FileName, bool DryRun = true)
    : ICommand<ImportArtifactsResultDto>;

public record ImportArtifactsResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<ArtifactRowErrorDto> Errors);

public record ArtifactRowErrorDto(
    int RowNumber,
    string LogicalArtifactId,   // e.g. "SITE-001" — helps the data team find the row in the sheet
    List<string> Errors);

public sealed class ImportArtifactsHandler(
    IUnitOfWork unitOfWork,
    ISiteQueryService siteQueryService,
    ICsvFileParser csvParser) : ICommandHandler<ImportArtifactsCommand, ImportArtifactsResultDto>
{
    private readonly ArtifactCsvRowValidator _rowValidator = new();

    public async Task<Result<ImportArtifactsResultDto>> HandleAsync(ImportArtifactsCommand command, CancellationToken ct)
    {
        List<ArtifactCsvRowDto> rows;
        try
        {
            rows = csvParser.ParseCsv<ArtifactCsvRowDto>(command.CsvFile);
        }
        catch (Exception ex)
        {
            return Result.Failure<ImportArtifactsResultDto>(
                ImportErrors.CsvParsingFailed(ex.Message));
        }

        // Load all sites into a name→Guid dictionary (one DB round-trip)
        var sites = await siteQueryService.GetAsync(
            filters: new SiteFilters(null, null, null, null, null),
            paging: new PagingParameters(Page: 1, PageSize: int.MaxValue),
            cancellationToken: ct);

        var siteMap = sites.Data.ToDictionary(
            c => c.Name,
            c => c.Id,
            StringComparer.OrdinalIgnoreCase);

        var errors = new List<ArtifactRowErrorDto>();
        var validArtifacts = new List<Artifact>();

        for (int i = 0; i < rows.Count; i++)
        {
            var row = rows[i];
            int rowNumber = i + 2; // +2: row 1 is the header
            var rowErrors = new List<string>();

            // --- FluentValidation ---
            var validation = await _rowValidator.ValidateAsync(row, ct);
            if (!validation.IsValid)
                rowErrors.AddRange(validation.Errors.Select(e => e.ErrorMessage));

            // --- Artifact resolution ---
            if (!siteMap.TryGetValue(row.SiteName, out var siteId))
            {
                if (!row.SiteName.Equals("Unknown", StringComparison.OrdinalIgnoreCase))
                    rowErrors.Add($"Site not found in database: '{row.SiteName}'. Check the site name spelling in the sheet.");
            }

            if (rowErrors.Count > 0)
            {
                errors.Add(new ArtifactRowErrorDto(rowNumber, row.ArtifactId, rowErrors));
                continue;
            }
            
            var artifactResult = MapToDomain(row, siteId == default ? null : siteId);
            if (artifactResult.Failed)
            {
                errors.Add(new ArtifactRowErrorDto(rowNumber, row.ArtifactId, [artifactResult.Error.Message]));
                continue;
            }

            validArtifacts.Add(artifactResult.Value);
        }

        // Batch insert — only valid rows reach here
        if (validArtifacts.Count == rows.Count)
        {
            await unitOfWork.Artifacts.AddRangeAsync(validArtifacts, ct);
        }
        else
        {
            // Failed: don't save any rows if there are errors, to avoid partial imports and simplify error correction for the data team
            return new ImportArtifactsResultDto(
                TotalRows: rows.Count,
                SuccessCount: 0,
                FailureCount: errors.Count,
                Errors: errors);
        }

        // if (validArtifacts.Count > 0)
        // {
        //     await unitOfWork.Artifacts.AddRangeAsync(validArtifacts, ct);
        // }

        if (!command.DryRun)
        {
            await unitOfWork.SaveChangesAsync(ct);
        }

        return Result.Success(new ImportArtifactsResultDto(
            TotalRows: rows.Count,
            SuccessCount: validArtifacts.Count,
            FailureCount: errors.Count,
            Errors: errors));
    }

    private static Result<Artifact> MapToDomain(ArtifactCsvRowDto row, Guid? siteId)
    {
        // Enums — already validated, so Parse is safe
        var type   = Enum.Parse<ArtifactType>(row.Type, ignoreCase: true);

        // --- Create the artifact (English content baked in via Artifact.Create) ---
        var artifactResult = Artifact.Create(
            siteId:                   siteId,
            name:                     row.NameEn,
            description:              row.DescriptionEn,
            type:                     type,
            displayOrder:            row.DisplayOrder
            );

        if (artifactResult.Failed) return artifactResult;
        var artifact = artifactResult.Value;

        // --- Arabic localized content ---
        var arResult = artifact.AddLocalizedContent(
            LanguageCode.Arabic,
            row.NameAr,
            row.DescriptionAr);
        if (arResult.Failed) return arResult.To<Artifact>();

        return artifact;
    }
}

public sealed record ArtifactCsvRowDto
{
    // Logical ID — used only in error messages, never stored in DB
    public string ArtifactId { get; init; } = null!;

    // Site English name — resolved to Guid via DB lookup
    public string SiteName { get; init; } = null!;

    public string NameEn { get; init; } = null!;
    public string NameAr { get; init; } = null!;

    public string DescriptionEn { get; init; } = null!;
    public string DescriptionAr { get; init; } = null!;

    public string Type { get; init; } = null!;      // ArtifactType enum

    public int DisplayOrder { get; init; }
}

