using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Queries.Attractions.Validators;

internal class GetAttractionsByTypeQueryValidator : AbstractValidator<GetAttractionsByTypeQuery>
{
    public GetAttractionsByTypeQueryValidator()
    {
        RuleFor(x => x.Type)
            .IsInEnum();
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