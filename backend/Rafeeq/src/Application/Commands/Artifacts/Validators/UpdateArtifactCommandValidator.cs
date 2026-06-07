using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.ArtifactAggregate;
using FluentValidation;

namespace Application.Commands.Artifacts.Validators;

internal sealed class UpdateArtifactCommandValidator : AbstractValidator<UpdateArtifactCommand>
{
    public UpdateArtifactCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.SiteId)
            .Must(id => !id.HasValue || id.Value != Guid.Empty)
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);

        RuleFor(x => x.DisplayOrder)
            .GreaterThan(0)
            .WithMessage(errors[ArtifactErrors.NegativeDisplayOrder.Code]);
        
        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage(errors[ValidationErrors.InvalidEnumValue.Code]);
    }
}
