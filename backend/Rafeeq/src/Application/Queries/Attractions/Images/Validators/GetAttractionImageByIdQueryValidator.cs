using Application.Common.Interfaces.Localization;
using Domain.Entities.AttractionAggregate;
using FluentValidation;

namespace Application.Queries.Attractions.Images.Validators;

internal sealed class GetAttractionImageByIdQueryValidator : AbstractValidator<GetAttractionImageByIdQuery>
{
    public GetAttractionImageByIdQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.AttractionId)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);

        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.ImageNotFound.Code]);
    }
}
