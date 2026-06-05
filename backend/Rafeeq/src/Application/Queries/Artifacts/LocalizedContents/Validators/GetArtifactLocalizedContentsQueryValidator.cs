using Domain.Entities.ArtifactAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Artifacts.LocalizedContents.Validators;

internal sealed class GetArtifactLocalizedContentsQueryValidator : AbstractValidator<GetArtifactLocalizedContentsQuery>
{
    public GetArtifactLocalizedContentsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.ArtifactId)
            .NotEmpty()
            .WithMessage(errors[ArtifactErrors.IdRequired.Code]);
    }
}
