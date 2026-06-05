using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Artifacts.Validators;

internal sealed class GetAllArtifactsQueryValidator : AbstractValidator<GetAllArtifactsQuery>
{
    public GetAllArtifactsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new PagingParametersValidator(errors));
    }
}
