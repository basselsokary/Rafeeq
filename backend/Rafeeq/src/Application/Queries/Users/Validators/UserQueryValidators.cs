using Application.Queries.Common.Validators;
using FluentValidation;

namespace Application.Queries.Users.Validators;

internal class GetAllUsersQueryValidator : AbstractValidator<GetAllUsersQuery>
{
    public GetAllUsersQueryValidator()
    {
        RuleFor(x => x.SearchTerm)
            .Must(searchTerm => searchTerm == null || !string.IsNullOrWhiteSpace(searchTerm))
            .WithMessage("Search term cannot be whitespace.");

        RuleFor(x => x.Status)
            .IsInEnum();

        RuleFor(x => x.Paging)
            .SetValidator(new PagingParametersValidator())
            .When(x => x.Paging is not null);
    }
}
