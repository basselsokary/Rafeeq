using Application.Commands.Sites.Images;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Validators;

internal class AddSiteImagesCommandValidator : AbstractValidator<AddSiteImagesCommand>
{
    public AddSiteImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage(SiteErrors.ImageUrlRequired.Message);
    }
}

internal class RemoveSiteImagesCommandValidator : AbstractValidator<RemoveSiteImagesCommand>
{
    public RemoveSiteImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.ImageId)
            .NotEmpty()
            .WithMessage(SiteErrors.ImageUrlRequired.Message);
    }
}

