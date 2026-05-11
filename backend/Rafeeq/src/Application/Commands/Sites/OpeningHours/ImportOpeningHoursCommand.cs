using Application.Commands.Sites.OpeningHours.Validators;
using Application.Common.Interfaces.Services;
using Application.Common.Models;
using Domain.Common.Interfaces;
using Domain.Enums;
using Domain.ValueObjects;

namespace Application.Commands.Sites.OpeningHours;

public record ImportOpeningHoursCommand(Stream CsvFile, string FileName, bool DryRun = true)
    : ICommand<ImportOpeningHoursResultDto>;

public record ImportOpeningHoursResultDto(
    int TotalRows,
    int SuccessCount,
    int FailureCount,
    List<OpeningHourRowErrorDto> Errors);

public record OpeningHourRowErrorDto(
    int RowNumber,
    string LogicalOpeningHourId,
    List<string> Errors);

public sealed class ImportOpeningHoursHandler(
    IUnitOfWork unitOfWork,
    ICsvFileParser csvParser) : ICommandHandler<ImportOpeningHoursCommand, ImportOpeningHoursResultDto>
{
    private const string AllDays = "ALLDAYS";

    private readonly OpeningHourCsvRowValidator _rowValidator = new();

    public async Task<Result<ImportOpeningHoursResultDto>> HandleAsync(
        ImportOpeningHoursCommand command,
        CancellationToken ct)
    {
        // ── 1. Parse CSV ──────────────────────────────────────────────────────
        List<OpeningHourCsvRowDto> rows;
        try
        {
            rows = csvParser.ParseCsv<OpeningHourCsvRowDto>(command.CsvFile);
        }
        catch (Exception ex)
        {
            return Result.Failure<ImportOpeningHoursResultDto>(
                ImportErrors.CsvParsingFailed(ex.Message));
        }

        // ── 2. Load all sites (one DB round-trip) ─────────────────────────────
        var sites = await unitOfWork.Sites.GetAllWithOpeningHoursAsync(ct);

        // name → Site entity (we need the full entity to call AddOpeningHour)
        var siteMap = sites.ToDictionary(
            s => s.LocalizedContents.Where(c => c.Language == LanguageCode.English).Select(c => c.Name).FirstOrDefault()!,
            s => s,
            StringComparer.OrdinalIgnoreCase);

        // ── 3. Validate + map each row ────────────────────────────────────────
        var errors = new List<OpeningHourRowErrorDto>();

        // Track expanded row count (ALLDAYS = 7 logical entries per CSV row)
        int totalExpanded = 0;
        int successExpanded = 0;

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
            if (!siteMap.TryGetValue(row.SiteName.Trim(), out var site))
                rowErrors.Add($"Site not found in database: '{row.SiteName}'. Check the site name spelling in the sheet.");

            if (rowErrors.Count > 0)
            {
                errors.Add(new OpeningHourRowErrorDto(rowNumber, row.OpeningHourId, rowErrors));
                // Still count the expanded rows for reporting
                totalExpanded += IsAllDays(row.Day) ? 7 : 1;
                continue;
            }

            // --- Build the TimeRange (shared across all days if ALLDAYS) ---
            // When IsClosed = true, StartTime/EndTime are empty — use midnight sentinel
            var timeRangeResult = BuildTimeRange(row);
            if (timeRangeResult.Failed)
            {
                errors.Add(new OpeningHourRowErrorDto(rowNumber, row.OpeningHourId,
                    [timeRangeResult.Error.Message]));
                totalExpanded += IsAllDays(row.Day) ? 7 : 1;
                continue;
            }

            var timeRange = timeRangeResult.Value;

            // --- Expand ALLDAYS into individual WeekDay entries ---
            var days = IsAllDays(row.Day)
                ? Enum.GetValues<WeekDay>()
                : [Enum.Parse<WeekDay>(row.Day, ignoreCase: true)];

            totalExpanded += days.Length;

            bool rowHadError = false;
            foreach (var day in days)
            {
                var addResult = site!.AddOpeningHour(day, timeRange, row.IsClosed);
                if (addResult.Failed)
                {
                    errors.Add(new OpeningHourRowErrorDto(rowNumber, row.OpeningHourId,
                        [$"Failed to add opening hour for {day}: {addResult.Error.Message}"]));
                    rowHadError = true;
                }
            }

            if (!rowHadError)
            {
                successExpanded += days.Length;
            }
        }

        // ── 4. All-or-nothing: only persist if every expanded entry is valid ───
        if (successExpanded != totalExpanded)
        {
            return new ImportOpeningHoursResultDto(
                TotalRows: totalExpanded,
                SuccessCount: 0,
                FailureCount: errors.Count,
                Errors: errors);
        }

        // Sites are already tracked by EF Core — updating them marks them as modified.
        // No explicit AddRange needed; SaveChanges picks up the mutations.
        if (!command.DryRun)
            await unitOfWork.SaveChangesAsync(ct);

        return Result.Success(new ImportOpeningHoursResultDto(
            TotalRows: totalExpanded,
            SuccessCount: successExpanded,
            FailureCount: errors.Count,
            Errors: errors));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private static bool IsAllDays(string day) =>
        day.Equals(AllDays, StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Builds a <see cref="TimeRange"/> from the row.
    /// When <see cref="OpeningHourCsvRowDto.IsClosed"/> is true, times are empty
    /// in the sheet — we use a zero-duration midnight sentinel (00:00–00:01)
    /// so the value object constraint (start &lt; end) is satisfied, while
    /// the <see cref="OpeningHour.IsClosed"/> flag carries the real meaning.
    /// </summary>
    private static Result<TimeRange> BuildTimeRange(OpeningHourCsvRowDto row)
    {
        if (row.IsClosed)
            return TimeRange.Create(TimeOnly.MinValue, TimeOnly.MaxValue);

        // Validator already confirmed these parse — safe to use directly
        var start = TimeOnly.Parse(row.StartTime!);
        var end   = TimeOnly.Parse(row.EndTime!);

        return TimeRange.Create(start, end, row.IsOvernight);
    }
}

public sealed record OpeningHourCsvRowDto
{
    // Logical ID — used only in error messages, never stored in DB
    public string OpeningHourId { get; init; } = null!;

    // Not used for resolution (SiteName is), but kept for traceability
    public string ParentSiteId { get; init; } = null!;

    // Site English name — resolved to Site entity via DB lookup
    public string SiteName { get; init; } = null!;

    // "ALLDAYS" or a WeekDay enum name (e.g. "Monday")
    public string Day { get; init; } = null!;

    // Nullable — empty when IsClosed = true
    public string? StartTime { get; init; }
    public string? EndTime { get; init; }

    public bool IsOvernight { get; init; }
    public bool IsClosed { get; init; }
}