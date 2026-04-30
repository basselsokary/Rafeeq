using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Images.Validators;

internal sealed class RemoveSiteImagesCommandValidator : AbstractValidator<RemoveSiteImagesCommand>
{
    public RemoveSiteImagesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
        
        RuleFor(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);
        
        RuleForEach(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.ImageIdRequired.Code]);
    }
}

