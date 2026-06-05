using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.ArtifactAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Artifact;

namespace Application.Commands.Artifacts.Validators;

internal sealed class CreateArtifactCommandValidator : AbstractValidator<CreateArtifactCommand>
{
    public CreateArtifactCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type.ToString()));

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(errors[ArtifactErrors.NegativeDisplayOrder.Code]);

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
