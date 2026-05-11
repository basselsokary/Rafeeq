using FluentValidation;

namespace Application.Commands.Cities.Validators;

public sealed class ImportCitiesCommandValidator : AbstractValidator<ImportCitiesCommand>
{
    public ImportCitiesCommandValidator()
    {
        RuleFor(x => x.CsvFile)
            .NotNull()
            .WithMessage("A CSV file is required.");
            
        RuleFor(x => x.FileName)
            .Must(f => Path.GetExtension(f).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("The uploaded file must be a .csv file.");
    }
}

public sealed class CityCsvRowValidator : AbstractValidator<CityCsvRowDto>
{
    public CityCsvRowValidator()
    {
        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("City Name (English) is required.")
            .MaximumLength(200);

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("City Name (Localized) is required.")
            .MaximumLength(200);

        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithMessage("Description (English) is required.");

        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithMessage("Description (Localized) is required.");

        RuleFor(x => x.Latitude)
            .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

        RuleFor(x => x.Longitude)
            .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0).WithMessage("Display Order must be a non-negative integer.");
    }
}
