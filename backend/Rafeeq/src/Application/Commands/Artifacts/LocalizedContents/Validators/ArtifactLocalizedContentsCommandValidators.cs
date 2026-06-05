using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.ArtifactAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Artifact;

namespace Application.Commands.Artifacts.LocalizedContents.Validators;

internal sealed class AddArtifactLocalizedContentCommandValidator : AbstractValidator<AddArtifactLocalizedContentsCommand>
{
    public AddArtifactLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddArtifactLocalizedContentsDtoCommandValidator(errors));
    }
}

internal sealed class AddArtifactLocalizedContentsDtoCommandValidator : AbstractValidator<AddArtifactLocalizedContentsDtoCommand>
{
    public AddArtifactLocalizedContentsDtoCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Language.ToString()));

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(errors[ArtifactErrors.NameRequired.Code])
            .MaximumLength(MaxNameLength)
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code, MaxNameLength));

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(errors[ArtifactErrors.DescriptionRequired.Code])
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code, MaxDescriptionLength));
    }
}
