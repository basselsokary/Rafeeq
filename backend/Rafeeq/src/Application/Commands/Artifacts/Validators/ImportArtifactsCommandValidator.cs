using Domain.Enums;
using FluentValidation;

namespace Application.Commands.Artifacts.Validators;

public sealed class ImportArtifactsCommandValidator : AbstractValidator<ImportArtifactsCommand>
{
    public ImportArtifactsCommandValidator()
    {
        RuleFor(x => x.CsvFile)
            .NotNull()
            .WithMessage("A CSV file is required.");
            
        RuleFor(x => x.FileName)
            .Must(f => Path.GetExtension(f).Equals(".csv", StringComparison.OrdinalIgnoreCase))
            .WithMessage("The uploaded file must be a .csv file.");
    }
}

public sealed class ArtifactCsvRowValidator : AbstractValidator<ArtifactCsvRowDto>
{
    public ArtifactCsvRowValidator()
    {
        RuleFor(x => x.SiteName)
            .NotEmpty().WithMessage("Parent Site ID (site name) is required.");

        RuleFor(x => x.NameEn)
            .NotEmpty().WithMessage("Artifact Name (English) is required.")
            .MaximumLength(200);

        RuleFor(x => x.NameAr)
            .NotEmpty().WithMessage("Artifact Name (Localized) is required.")
            .MaximumLength(200);

        RuleFor(x => x.DescriptionEn)
            .NotEmpty().WithMessage("Description (English) is required.");

        RuleFor(x => x.DescriptionAr)
            .NotEmpty().WithMessage("Description (Localized) is required.");

        RuleFor(x => x.Type)
            .NotEmpty()
            .Must(t => Enum.TryParse<ArtifactType>(t, ignoreCase: true, out _))
            .WithMessage(x => $"Unknown Type value: '{x.Type}'. Valid values: {string.Join(", ", Enum.GetNames<ArtifactType>())}");
        
        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Display Order must be a non-negative integer.");
    }
}
