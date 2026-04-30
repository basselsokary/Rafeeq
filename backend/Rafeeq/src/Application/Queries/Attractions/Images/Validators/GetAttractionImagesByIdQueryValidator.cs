using Application.Common.Interfaces.Localization;
using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Queries.Attractions.Images.Validators;

internal sealed class GetAttractionImagesByIdQueryValidator : AbstractValidator<GetAttractionImagesByIdQuery>
{
    public GetAttractionImagesByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);
    }
}
