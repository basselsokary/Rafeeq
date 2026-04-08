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
        
        RuleFor(x => x.Images)
            .NotEmpty();

        RuleForEach(x => x.Images)
            .SetValidator(new AddSiteImageDtoValidator());
    }
}

internal class AddSiteImageDtoValidator : AbstractValidator<AddSiteImageDto>
{
    public AddSiteImageDtoValidator()
    {
        RuleFor(x => x.ImageUrl)
            .NotEmpty()
            .WithMessage(SiteErrors.ImageUrlRequired.Message);

        RuleFor(x => x.DisplayOrder)
            .GreaterThanOrEqualTo(0)
            .WithMessage(SiteErrors.NegativeDisplayOrder.Message);

        RuleFor(x => x.Caption)
            .Must(caption => caption == null || !string.IsNullOrWhiteSpace(caption));
    }
}

internal class RemoveSiteImagesCommandValidator : AbstractValidator<RemoveSiteImagesCommand>
{
    public RemoveSiteImagesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(SiteErrors.ImageIdRequired.Message);
        
        RuleForEach(x => x.ImageIds)
            .NotEmpty()
            .WithMessage(SiteErrors.ImageIdRequired.Message);
    }
}

