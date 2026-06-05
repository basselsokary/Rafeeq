using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.ArtifactAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Artifact;

namespace Application.Commands.Artifacts.LocalizedContents.Validators;

internal sealed class UpdateArtifactLocalizedContentCommandValidator : AbstractValidator<UpdateArtifactLocalizedContentCommand>
{
    public UpdateArtifactLocalizedContentCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.LocalizedContents)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new UpdateArtifactLocalizedContentsDtoValidator(errors));
    }
}

internal sealed class UpdateArtifactLocalizedContentsDtoValidator : AbstractValidator<UpdateArtifactLocalizedContentsDtoCommand>
{
    public UpdateArtifactLocalizedContentsDtoValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Language)
            .IsInEnum()
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);

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
