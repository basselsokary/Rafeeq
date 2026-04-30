using FluentValidation;
using Domain.Entities.SiteAggregate;
using Application.Common.Interfaces.Localization;

namespace Application.Queries.Sites.Validators;

internal sealed class GetSiteByIdForAdminQueryValidator : AbstractValidator<GetSiteByIdForAdminQuery>
{
    public GetSiteByIdForAdminQueryValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
    }
}

