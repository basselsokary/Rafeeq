using Application.Common.Interfaces.Localization;
using Domain.Entities.ArtifactAggregate;
using FluentValidation;

namespace Application.Queries.Artifacts.Images.Validators;

internal sealed class GetArtifactImagesByIdQueryValidator : AbstractValidator<GetArtifactImagesByIdQuery>
{
    public GetArtifactImagesByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.ArtifactId)
            .NotEmpty()
            .WithMessage(errors[ArtifactErrors.IdRequired.Code]);
    }
}
