using Application.Common.Interfaces.Localization;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Images.Validators;

internal sealed class SetMainSiteImageCommandValidator : AbstractValidator<SetMainSiteImageCommand>
{
    public SetMainSiteImageCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
        
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.ImageIdRequired.Code]);
    }
}

