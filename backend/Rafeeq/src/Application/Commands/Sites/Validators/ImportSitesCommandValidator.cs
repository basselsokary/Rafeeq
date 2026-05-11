using Domain.Enums;
using FluentValidation;

namespace Application.Commands.Sites.Validators;

public sealed class ImportSitesCommandValidator : AbstractValidator<ImportSitesCommand>
{
    public ImportSitesCommandValidator()
    {
        RuleFor(x => x.CsvFile)
            .NotNull()
            .WithMessage("A CSV file is required.");
            
        RuleFor(x => x.FileName)
            .Must(f => Path.GetExtension(f).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("The uploaded file must be a .csv file.");
    }
}

public sealed class SiteCsvRowValidator : AbstractValidator<SiteCsvRowDto>
{
    public SiteCsvRowValidator()
    {
        RuleFor(x => x.CityName)
            .NotEmpty().WithMessage("Parent City ID (city name) is required.");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("Site Name (English) is required.")
            .MaximumLength(200);

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Site Name (Localized) is required.")
            .MaximumLength(200);

        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithMessage("Description (English) is required.");

        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithMessage("Description (Localized) is required.");

        RuleFor(x => x.AddressEn)
            .NotEmpty().WithMessage("Address (English) is required.");

        RuleFor(x => x.AddressAr)
            .NotEmpty().WithMessage("Address (Localized) is required.");

        RuleFor(x => x.Status)
            .NotEmpty()
            .Must(s => Enum.TryParse<SiteStatus>(s, ignoreCase: true, out _))
            .WithMessage(x => $"Unknown Status value: '{x.Status}'. Valid values: {string.Join(", ", Enum.GetNames<SiteStatus>())}");

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => Enum.TryParse<SiteType>(t, ignoreCase: true, out _))
            .WithMessage(x => $"Unknown Type value: '{x.Type}'. Valid values: {string.Join(", ", Enum.GetNames<SiteType>())}");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");

        RuleFor(x => x.EstimatedDurationMinutes)
            .GreaterThan(0).WithMessage("Estimated Duration Minutes must be greater than 0.");

        // Entry Fee format: "700, 60"
        RuleFor(x => x.EntryFee)
            .Must(BeValidEntryFeeFormat)
            .When(x => !string.IsNullOrWhiteSpace(x.EntryFee) && !x.EntryFee.Trim().Equals("null", StringComparison.OrdinalIgnoreCase))
            .WithMessage("Entry Fee must be in format 'ForeignerFee, EgyptianFee' (e.g. '700, 60').");

        // IsFree=TRUE must not have an Entry Fee
        RuleFor(x => x.EntryFee)
            .Must(x => string.IsNullOrWhiteSpace(x) || x.Equals("null", StringComparison.OrdinalIgnoreCase))
            .When(x => x.IsFree)
            .WithMessage("Entry Fee must be empty when 'Is Free?' is TRUE.");

        // IsFeatured and IsHiddenGem are mutually exclusive
        RuleFor(x => x.IsHiddenGem)
            .Equal(false)
            .When(x => x.IsFeatured)
            .WithMessage("A site cannot be both Featured and a Hidden Gem.");

        RuleFor(x => x.Facilities)
            .Must(BeValidFacilities)
            .When(x => !string.IsNullOrWhiteSpace(x.Facilities))
            .WithMessage(x => $"One or more invalid Facility values in: '{x.Facilities}'. Valid values: {string.Join(", ", Enum.GetNames<FacilityType>())}");
    }

    private static bool BeValidEntryFeeFormat(string? fee)
    {
        if (string.IsNullOrWhiteSpace(fee)) return true;
        var parts = fee.Split(',');
        return parts.Length == 2
               && decimal.TryParse(parts[1].Trim(), out var egyptian) && egyptian >= 0;
    }

    private static bool BeValidFacilities(string facilities)
    {
        return facilities
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .All(f => Enum.TryParse<FacilityType>(f, ignoreCase: true, out _));
    }
}
