using Domain.Entities.ArtifactAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Artifacts.Validators;

internal sealed class GetArtifactsBySiteIdQueryValidator : AbstractValidator<GetArtifactsBySiteIdQuery>
{
    public GetArtifactsBySiteIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[ArtifactErrors.SiteIdRequired.Code]);

        RuleFor(x => x.Type)
            .IsInEnum()
            .When(x => x.Type != null)
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Type?.ToString()!));

        RuleFor(x => x.SearchTerm)
            .MaximumLength(100)
            .When(x => !string.IsNullOrEmpty(x.SearchTerm))
            .WithMessage(errors.Format(ValidationErrors.MaximumLengthExceeded.Code));
    }
}
