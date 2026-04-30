using Domain.Entities.AttractionAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Attractions.Validators;

internal sealed class MarkAttractionAsFeaturedCommandValidator : AbstractValidator<MarkAttractionAsFeaturedCommand>
{
    public MarkAttractionAsFeaturedCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[AttractionErrors.IdRequired.Code]);
    }
}
