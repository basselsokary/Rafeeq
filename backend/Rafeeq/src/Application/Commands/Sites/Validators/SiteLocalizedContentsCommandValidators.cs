using Application.Commands.Sites.LocalizedContents;
using Domain.Entities.SiteAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Site;

namespace Application.Commands.Sites.Validators;

internal class AddSiteLocalizedContentCommandValidator : AbstractValidator<AddSiteLocalizedContentsCommand>
{
    public AddSiteLocalizedContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.LocalizedContents)
            .NotEmpty();

        RuleForEach(x => x.LocalizedContents)
            .SetValidator(new AddSiteLocalizedContentsDtoCommandValidator());
    }
}

internal class AddSiteLocalizedContentsDtoCommandValidator : AbstractValidator<AddSiteLocalizedContentsDtoCommand>
{
    public AddSiteLocalizedContentsDtoCommandValidator()
    {
        RuleFor(x => x.Language)
            .IsInEnum();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(SiteErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SiteErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength);
    }
}

internal class UpdateSiteLocalizedContentCommandValidator : AbstractValidator<UpdateSiteLocalizedContentCommand>
{
    public UpdateSiteLocalizedContentCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);

        RuleFor(x => x.ContentId)
            .NotEmpty();

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(SiteErrors.NameRequired.Message)
            .MaximumLength(MaxNameLength);

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage(SiteErrors.DescriptionRequired.Message)
            .MaximumLength(MaxDescriptionLength);
    }
}

