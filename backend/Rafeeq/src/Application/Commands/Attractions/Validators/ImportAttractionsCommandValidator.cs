using Domain.Enums;
using FluentValidation;

namespace Application.Commands.Attractions.Validators;

public sealed class ImportAttractionsCommandValidator : AbstractValidator<ImportAttractionsCommand>
{
    public ImportAttractionsCommandValidator()
    {
        RuleFor(x => x.CsvFile)
            .NotNull()
            .WithMessage("A CSV file is required.");
            
        RuleFor(x => x.FileName)
            .Must(f => Path.GetExtension(f).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("The uploaded file must be a .csv file.");
    }
}

public sealed class AttractionCsvRowValidator : AbstractValidator<AttractionCsvRowDto>
{
    public AttractionCsvRowValidator()
    {
        RuleFor(x => x.SiteName)
            .NotEmpty().WithMessage("Parent Site Name (site name) is required.");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("Name (English) is required.")
            .MaximumLength(200);

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Name (Localized) is required.")
            .MaximumLength(200);

        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithMessage("Description (English) is required.");

        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithMessage("Description (Localized) is required.");

        RuleFor(x => x.LocationGuidEn)
            .NotEmpty().WithMessage("Address (English) is required.");

        RuleFor(x => x.LocationGuidAr)
            .NotEmpty().WithMessage("Address (Localized) is required.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => Enum.TryParse<AttractionType>(t, ignoreCase: true, out _))
            .WithMessage(x => $"Unknown Type value: '{x.Type}'. Valid values: {string.Join(", ", Enum.GetNames<AttractionType>())}");

        // RuleFor(x => x)
        //     .Must(x => 
        //     {
        //         bool latParsed = double.TryParse(x.Latitude, out var lat);
        //         bool lonParsed = double.TryParse(x.Longitude, out var lon);
        //         return latParsed && lonParsed && BeValidLocation(lat, lon); // Both present and valid
        //     })
        //     .When(x => double.TryParse(x.Latitude, out var lat) && double.TryParse(x.Longitude, out var lon))
        //     .WithMessage(x => $"Invalid Latitude and/or Longitude values: ({x.Latitude}, {x.Longitude}). Latitude must be between -90 and 90. Longitude must be between -180 and 180.");
        
        RuleFor(x => x.HistoricalPeriods)
            .Must(BeValidHistoricalPeriods)
            .When(x => !string.IsNullOrWhiteSpace(x.HistoricalPeriods))
            .WithMessage(x => $"One or more invalid Facility values in: '{x.HistoricalPeriods}'. Valid values: {string.Join(", ", Enum.GetNames<FacilityType>())}");
    }

    private static bool BeValidLocation(double lat, double lon)
    {
        return lat >= -90 && lat <= 90 && lon >= -180 && lon <= 180;
    }

    private static bool BeValidHistoricalPeriods(string historicalPeriods)
    {
        return historicalPeriods
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .All(f => Enum.TryParse<HistoricalPeriod>(f, ignoreCase: true, out _));
    }
}
