using Domain.Entities.SiteAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sites.LocalizedContents.Validators;

internal sealed class GetSiteLocalizedContentsQueryValidator : AbstractValidator<GetSiteLocalizedContentsQuery>
{
    public GetSiteLocalizedContentsQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
    }
}
