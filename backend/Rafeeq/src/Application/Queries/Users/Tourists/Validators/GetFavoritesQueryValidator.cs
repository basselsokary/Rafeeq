using FluentValidation;
using Application.Queries.Common.Validators;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Queries.Users.Tourists.Validators;

internal sealed class GetFavoritesQueryValidator : AbstractValidator<GetFavoritesQuery>
{
    public GetFavoritesQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Paging)
            .SetValidator(new PagingParametersValidator(errors))
            .When(x => x.Paging is not null)
            .WithMessage(errors[ValidationErrors.ValueRequired.Code]);
    }
}
