using FluentValidation;
using Application.Queries.Common.Validators;

namespace Application.Queries.Users.Tourists.Validators;

internal class GetFavoritesQueryValidator : AbstractValidator<GetFavoritesQuery>
{
    public GetFavoritesQueryValidator()
    {
        RuleFor(x => x.Paging)
            .SetValidator(new PagingParametersValidator())
            .When(x => x.Paging is not null);
    }
}

