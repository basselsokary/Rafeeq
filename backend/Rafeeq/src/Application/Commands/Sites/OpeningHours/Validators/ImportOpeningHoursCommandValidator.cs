using Domain.Enums;
using FluentValidation;

namespace Application.Commands.Sites.OpeningHours.Validators;

public sealed class ImportOpeningHoursCommandValidator
    : AbstractValidator<ImportOpeningHoursCommand>
{
    public ImportOpeningHoursCommandValidator()
    {
        RuleFor(x => x.CsvFile)
            .NotNull()
            .WithMessage("A CSV file is required.");

        RuleFor(x => x.FileName)
            .Must(f => Path.GetExtension(f).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("The uploaded file must be a .csv file.");
    }
}


// ─────────────────────────────────────────────────────────────────────────────
// Row Validator
// ─────────────────────────────────────────────────────────────────────────────

public sealed class OpeningHourCsvRowValidator : AbstractValidator<OpeningHourCsvRowDto>
{
    private const string AllDays = "ALLDAYS";

    public OpeningHourCsvRowValidator()
    {
        RuleFor(x => x.SiteName)
            .NotEmpty().WithMessage("Site Name is required.");

        RuleFor(x => x.Day)
            .NotEmpty().WithMessage("Day is required.")
            .Must(BeAValidDay)
            .WithMessage(x =>
                $"Unknown Day value: '{x.Day}'. " +
                $"Valid values: {AllDays}, {string.Join(", ", Enum.GetNames<WeekDay>())}");

        // StartTime and EndTime are only required when the site is NOT closed
        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start Time is required when Is Closed is FALSE.")
            .Must(t => TimeOnly.TryParse(t, out _)).WithMessage("Start Time is not a valid time (expected HH:mm:ss).")
            .When(x => !x.IsClosed);

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End Time is required when Is Closed is FALSE.")
            .Must(t => TimeOnly.TryParse(t, out _)).WithMessage("End Time is not a valid time (expected HH:mm:ss).")
            .When(x => !x.IsClosed);

        // Cross-field: StartTime must be before EndTime
        RuleFor(x => x)
            .Must(x =>
            {
                if (!TimeOnly.TryParse(x.StartTime, out var start)) return true; // already caught above
                if (!TimeOnly.TryParse(x.EndTime, out var end)) return true;     // already caught above
                return end >= start;
            })
            .WithMessage("Start Time must be before End Time.")
            .When(x => !x.IsClosed
                && !string.IsNullOrWhiteSpace(x.StartTime)
                && !string.IsNullOrWhiteSpace(x.EndTime)
                && !x.IsOvernight);
    }

    private static bool BeAValidDay(string value) =>
        value.Equals(AllDays, StringComparison.OrdinalIgnoreCase)
        || Enum.TryParse<WeekDay>(value, ignoreCase: true, out _);
}