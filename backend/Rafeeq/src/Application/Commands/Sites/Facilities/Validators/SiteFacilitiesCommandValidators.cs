using Application.Common.Interfaces.Localization;
using Domain.Common;
using Domain.Entities.SiteAggregate;
using FluentValidation;
using static Domain.Common.Constants.DomainConstants.Site;

namespace Application.Commands.Sites.Facilities.Validators;

internal sealed class AddSiteFacilitiesCommandValidator : AbstractValidator<AddSiteFacilitiesCommand>
{
    public AddSiteFacilitiesCommandValidator(IErrorLocalizer errors)
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