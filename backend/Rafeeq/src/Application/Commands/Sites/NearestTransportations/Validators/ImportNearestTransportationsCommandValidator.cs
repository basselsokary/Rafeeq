using Domain.Enums;
using FluentValidation;

namespace Application.Commands.Sites.NearestTransportations.Validators;

public sealed class ImportNearestTransportationsCommandValidator
    : AbstractValidator<ImportNearestTransportationsCommand>
{
    public ImportNearestTransportationsCommandValidator()
    {
        RuleFor(x => x.CsvFile)
            .NotNull()
            .WithMessage("A CSV file is required.");

        RuleFor(x => x.FileName)
            .Must(f => Path.GetExtension(f).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("The uploaded file must be a .csv file.");
    }
}

public sealed class NearestTransportationCsvRowValidator : AbstractValidator<NearestTransportationCsvRowDto>
{
    public NearestTransportationCsvRowValidator()
    {
        RuleFor(x => x.SiteName)
            .NotEmpty().WithMessage("Site Name is required.");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("Transport Name (English) is required.")
            .MaximumLength(200);

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Transport Name (Arabic) is required.")
            .MaximumLength(200);

        // Description and Address are NULLABLE — no NotEmpty rules.
        RuleFor(x => x.DescriptionEn)
            .MaximumLength(1000)
            .When(x => x.DescriptionEn is not null);

        RuleFor(x => x.DescriptionAr)
            .MaximumLength(1000)
            .When(x => x.DescriptionAr is not null);

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => Enum.TryParse<TransportationType>(t, ignoreCase: true, out _))
            .WithMessage(x =>
                $"Unknown Transport Type: '{x.Type}'. " +
                $"Valid values: {string.Join(", ", Enum.GetNames<TransportationType>())}");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90)
            .WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180)
            .WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.DistanceKm)
            .GreaterThan(0)
            .WithMessage("Distance to Site (Km) must be greater than 0.");

        // OperatingHours is optional, but if provided must be parseable
        RuleFor(x => x.OperatingHours)
            .Must(BeAValidTimeRange)
            .WithMessage("Operating Hours format is invalid. Expected format: 'HH:mm AM/PM - HH:mm AM/PM' (e.g. '05:00 AM - 01:00 AM').")
            .When(x => !string.IsNullOrWhiteSpace(x.OperatingHours));
    }

    private static bool BeAValidTimeRange(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;

        var parts = value.Split('-', 2, StringSplitOptions.TrimEntries);
        if (parts.Length != 2) return false;

        return TimeOnly.TryParse(parts[0], out _) && TimeOnly.TryParse(parts[1], out _);
    }
}