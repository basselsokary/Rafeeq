using Domain.Entities.SiteAggregate;
using FluentValidation;
using Application.Common.Interfaces.Localization;
using Domain.Common;

namespace Application.Commands.Sites.Validators;

internal sealed class UpdateSiteStatusCommandValidator : AbstractValidator<UpdateSiteStatusCommand>
{
    public UpdateSiteStatusCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
        
        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage(x => errors.Format(ValidationErrors.InvalidEnumValue.Code, x.Status.ToString()));
    }
}
