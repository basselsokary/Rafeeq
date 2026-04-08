using FluentValidation;
using Domain.Entities.CityAggregate;
using Application.Queries.Common.Validators;

namespace Application.Queries.Cities.Validators;

internal class GetCityByIdQueryValidator : AbstractValidator<GetCityByIdQuery>
{
    public GetCityByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(CityErrors.IdRequired.Message);
    }
}

internal class GetCityByIdForAdminQueryValidator : AbstractValidator<GetCityByIdForAdminQuery>
{
    public GetCityByIdForAdminQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(CityErrors.IdRequired.Message);
    }
}

internal class GetCitiesQueryValidator : AbstractValidator<GetCitiesQuery>
{
    public GetCitiesQueryValidator()
    {
        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());
    }
}