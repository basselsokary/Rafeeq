using Domain.Entities.AttractionAggregate;
using FluentValidation;
using Application.Queries.Common.Validators;

namespace Application.Queries.Attractions.Validators;

internal class GetAttractionsByTypeQueryValidator : AbstractValidator<GetAttractionsByTypeQuery>
{
    public GetAttractionsByTypeQueryValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(AttractionErrors.SiteIdRequired.Message);

        RuleFor(x => x.Type)
            .IsInEnum();

        RuleFor(x => x.Paging)
            .NotNull()
            .SetValidator(new PagingParametersValidator());
    }
}

internal class GetAttractionByIdQueryValidator : AbstractValidator<GetAttractionByIdQuery>
{
    public GetAttractionByIdQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
    }
}

internal class GetAttractionByIdForAdminQueryValidator : AbstractValidator<GetAttractionByIdForAdminQuery>
{
    public GetAttractionByIdForAdminQueryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
    }
}