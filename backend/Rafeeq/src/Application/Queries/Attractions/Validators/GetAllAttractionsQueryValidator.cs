using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Attractions.Validators;

internal sealed class GetAllAttractionsQueryValidator : AbstractValidator<GetAllAttractionsQuery>
{
    public GetAllAttractionsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .WithMessage(errors[ValidationErrors.ValueRequired.Code])
            .SetValidator(new PagingParametersValidator(errors));
    }
}
