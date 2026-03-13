using Application.Commands.Sites.LocalizedContents;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Validators;

internal class AddSiteLocalizedContentCommandValidator : AbstractValidator<AddSiteLocalizedContentCommand>
{
    public AddSiteLocalizedContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.Language)
            .IsInEnum();
        
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(SiteErrors.NameRequired.Message);
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SiteErrors.DescriptionRequired.Message);
    }
}

