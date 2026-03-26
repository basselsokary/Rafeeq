using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Queries.Sites.LocalizedContents.Validators;

internal class GetSiteLocalizedContentsQueryValidator : AbstractValidator<GetSiteLocalizedContentsQuery>
{
    public GetSiteLocalizedContentsQueryValidator()
    {
        RuleFor(x => x.SiteId)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
    }
}
