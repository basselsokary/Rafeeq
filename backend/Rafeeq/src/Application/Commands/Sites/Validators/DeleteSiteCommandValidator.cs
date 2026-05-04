using Domain.Entities.SiteAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;

namespace Application.Commands.Sites.Validators;

internal sealed class DeleteSiteCommandValidator : AbstractValidator<DeleteSiteCommand>
{
    public DeleteSiteCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
    }
}
