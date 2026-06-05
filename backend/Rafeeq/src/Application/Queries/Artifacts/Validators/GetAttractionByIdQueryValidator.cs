using Domain.Entities.ArtifactAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Artifacts.Validators;

internal sealed class GetArtifactByIdQueryValidator : AbstractValidator<GetArtifactByIdQuery>
{
    public GetArtifactByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[ArtifactErrors.IdRequired.Code]);
    }
}
