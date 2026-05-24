using Application.Common.Interfaces.Services;
using Domain.Common.Interfaces;

namespace Application.Commands.Sponsors;

public record ImportSponsorsCommand(Stream CsvFile, string FileName, bool DryRun = true)
    : ICommand<ImportSponsorsResultDto>;

public record ImportSponsorsResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<SponsorRowErrorDto> Errors);

public record SponsorRowErrorDto(
    int RowNumber,
    string LogicalSponsorId,   // e.g. "SITE-001" — helps the data team find the row in the sheet
    List<string> Errors);

public sealed class ImportSponsorsHandler(
    IUnitOfWork unitOfWork,
    ICsvFileParser csvParser) : ICommandHandler<ImportSponsorsCommand, ImportSponsorsResultDto>
{
    public Task<Result<ImportSponsorsResultDto>> HandleAsync(ImportSponsorsCommand command, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}

public sealed class SponsorCsvRowDto
{
    public string SponsorId { get; init; } = string.Empty;      // Stable human-readable seeding key (not a GUID)
    public string TitleEn { get; init; } = string.Empty;
    public string TitleAr { get; init; } = string.Empty;
    public string DescriptionEn { get; init; } = string.Empty;
    public string DescriptionAr { get; init; } = string.Empty;
    public string AddressEn { get; init; } = string.Empty;
    public string AddressAr { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;         // SponsorStatus enum name
    public string Type { get; init; } = string.Empty;           // SponsorType enum name
    public string Tier { get; init; } = string.Empty;           // SponsorTier enum name
    public double Latitude { get; init; }
    public double Longitude { get; init; }
    public string StartDate { get; init; } = string.Empty;      // yyyy-MM-dd
    public string EndDate { get; init; } = string.Empty;        // yyyy-MM-dd
    public string? WebsiteUrl { get; init; }
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
}