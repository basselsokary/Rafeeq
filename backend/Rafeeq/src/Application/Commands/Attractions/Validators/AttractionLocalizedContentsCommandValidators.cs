using Application.Commands.Attractions.LocalizedContents;
using Domain.Entities.AttractionAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Attraction;

namespace Application.Commands.Attractions.Validators;

internal class AddAttractionLocalizedContentCommandValidator : AbstractValidator<AddAttractionLocalizedContentsCommand>
{
    public AddAttractionLocalizedContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);
        
        RuleFor(x => x.LocalizedContents)
            .NotEmpty();

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddAttractionLocalizedContentsDtoCommandValidator());
    }
}

internal class AddAttractionLocalizedContentsDtoCommandValidator : AbstractValidator<AddAttractionLocalizedContentsDtoCommand>
{
    public AddAttractionLocalizedContentsDtoCommandValidator()
    {
        RuleFor(x => x.Language)
            .IsInEnum();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(AttractionErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength)
            .WithMessage(AttractionErrors.ExceededNameLength.Message);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(AttractionErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(AttractionErrors.ExceededDescriptionLength.Message);
    }
}

internal class UpdateAttractionLocalizedContentCommandValidator : AbstractValidator<UpdateAttractionLocalizedContentCommand>
{
    public UpdateAttractionLocalizedContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(AttractionErrors.IdRequired.Message);

        RuleFor(x => x.ContentId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(AttractionErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength)
            .WithMessage(AttractionErrors.ExceededNameLength.Message);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(AttractionErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength)
            .WithMessage(AttractionErrors.ExceededDescriptionLength.Message);
    }
}
