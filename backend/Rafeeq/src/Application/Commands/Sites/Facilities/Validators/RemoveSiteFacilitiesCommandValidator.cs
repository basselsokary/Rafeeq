using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Facilities.Validators;

internal sealed class RemoveSiteFacilitiesCommandValidator : AbstractValidator<RemoveSiteFacilitiesCommand>
{
    public RemoveSiteFacilitiesCommandValidator(IErrorLocalizer errors)
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(errors[SiteErrors.IdRequired.Code]);
    
        RuleFor(x => x.FacilityTypes)
            .NotEmpty()
            .WithMessage(errors[ValidationErrors.CollectionRequired.Code]);

        RuleForEach(x => x.FacilityTypes)
            .IsInEnum()
            .WithMessage(errors[SiteErrors.InvalidFacilityType.Code]);
    }
}
