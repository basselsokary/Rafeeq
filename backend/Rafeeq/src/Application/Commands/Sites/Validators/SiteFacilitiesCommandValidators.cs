using Application.Commands.Sites.Facilities;
using Domain.Entities.SiteAggregate;
using FluentValidation;

namespace Application.Commands.Sites.Validators;

internal class AddSiteFacilitiesCommandValidator : AbstractValidator<AddSiteFacilitiesCommand>
{
    public AddSiteFacilitiesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
    
        RuleFor(x => x.Facilities)
            .NotEmpty();

        RuleForEach(x => x.Facilities)
            .SetValidator(new AddSiteFacilitiesDtoCommandValidator());
    }
}

internal class AddSiteFacilitiesDtoCommandValidator : AbstractValidator<AddSiteFacilitiesDtoCommand>
{
    public AddSiteFacilitiesDtoCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty();

        RuleFor(x => x.FacilityName)
            .NotEmpty()
            .WithMessage(SiteErrors.NameRequired.Message);

        RuleFor(x => x.FacilityDescription)
            .NotEmpty()
            .WithMessage(SiteErrors.DescriptionRequired.Message);
    }
}

internal class RemoveSiteFacilitiesCommandValidator : AbstractValidator<RemoveSiteFacilitiesCommand>
{
    public RemoveSiteFacilitiesCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(SiteErrors.IdRequired.Message);
        
        RuleFor(x => x.FacilityIds)
            .NotEmpty()
            .WithMessage(SiteErrors.FacilityIdRequired.Message);
        
        RuleForEach(x => x.FacilityIds)
            .NotEmpty()
            .WithMessage(SiteErrors.FacilityIdRequired.Message);
    }
}
